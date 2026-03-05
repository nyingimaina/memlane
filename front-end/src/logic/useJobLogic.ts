import { useState, useEffect, useCallback } from 'react';
import { JobMetadata, JobStatusUpdate } from '@/models/Job';
import { JobRepository } from '@/repositories/JobRepository';
import { SignalRService } from '@/services/SignalRService';

export const useJobLogic = () => {
    const [jobs, setJobs] = useState<JobMetadata[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const fetchJobs = useCallback(async () => {
        setIsLoading(true);
        try {
            const data = await JobRepository.getAll();
            setJobs(data);
            setError(null);
        } catch (err) {
            console.error("Failed to fetch jobs:", err);
            setError("Failed to load jobs. Please check if the backend is running.");
        } finally {
            setIsLoading(false);
        }
    }, []);

    const handleStatusUpdate = useCallback((update: JobStatusUpdate) => {
        // Refresh full list if job finishes to get latest health score/run ID
        if (update.status === 'Completed' || update.status === 'Failed' || update.status === 'Skipped') {
            fetchJobs();
        } else {
            setJobs(prevJobs => prevJobs.map(job => 
                job.id === update.jobId 
                    ? { ...job, status: parseStatus(update.status), lastError: update.status === 'Failed' ? update.message : job.lastError } 
                    : job
            ));
        }
    }, [fetchJobs]);

    const parseStatus = (status: string) => {
        switch (status) {
            case 'Pending': return 0;
            case 'InProgress': return 1;
            case 'Completed': return 2;
            case 'Failed': return 3;
            case 'Skipped': return 4;
            default: return 0;
        }
    };

    useEffect(() => {
        const signalR = new SignalRService(handleStatusUpdate);
        signalR.start();

        fetchJobs();

        return () => {
            signalR.stop();
        };
    }, [fetchJobs, handleStatusUpdate]);

    const triggerJob = async (id: number) => {
        try {
            await JobRepository.trigger(id);
            // SignalR will handle status updates, but let's do a quick local optimistic update
            setJobs(prevJobs => prevJobs.map(job => 
                job.id === id ? { ...job, status: 1 } : job
            ));
        } catch (err) {
            console.error("Failed to trigger job:", err);
            setError("Failed to trigger job.");
        }
    };

    return {
        jobs,
        isLoading,
        error,
        fetchJobs,
        triggerJob
    };
};
