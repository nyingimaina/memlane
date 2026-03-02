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
        setJobs(prevJobs => prevJobs.map(job => 
            job.id === update.jobId 
                ? { ...job, status: parseStatus(update.status), lastError: update.status === 'Failed' ? update.message : job.lastError } 
                : job
        ));
    }, []);

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
        fetchJobs();

        const signalR = new SignalRService(handleStatusUpdate);
        signalR.start();

        return () => {
            signalR.stop();
        };
    }, [fetchJobs, handleStatusUpdate]);

    const triggerJob = async (id: number) => {
        try {
            await JobRepository.trigger(id);
            await fetchJobs(); // Refresh to show in progress
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
