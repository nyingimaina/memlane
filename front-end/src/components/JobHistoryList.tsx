'use client';

import React, { useEffect, useState } from 'react';
import { JobRun, JobStatus } from '@/models/Job';
import { JobRepository } from '@/repositories/JobRepository';
import { FaClock, FaCircleCheck, FaCircleXmark, FaCircleInfo, FaForwardStep } from 'react-icons/fa6';

interface JobHistoryListProps {
    jobId: number;
    onSelectRun: (run: JobRun) => void;
}

const JobHistoryList: React.FC<JobHistoryListProps> = ({ jobId, onSelectRun }) => {
    const [runs, setRuns] = useState<JobRun[]>([]);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        const fetchRuns = async () => {
            setIsLoading(true);
            try {
                const data = await JobRepository.getRuns(jobId);
                setRuns(data);
            } catch (err) {
                console.error("Failed to fetch runs", err);
            } finally {
                setIsLoading(false);
            }
        };
        fetchRuns();
    }, [jobId]);

    const getStatusIcon = (status: JobStatus) => {
        switch (status) {
            case JobStatus.Completed: return <FaCircleCheck color="var(--success)" />;
            case JobStatus.Failed: return <FaCircleXmark color="var(--danger)" />;
            case JobStatus.InProgress: return <FaClock className="status-pulse" color="var(--info)" />;
            case JobStatus.Skipped: return <FaForwardStep color="var(--secondary)" />;
            default: return <FaCircleInfo color="var(--info)" />;
        }
    };

    if (isLoading) return <div style={{ padding: '1rem' }}>Loading history...</div>;
    if (runs.length === 0) return <div style={{ padding: '1rem', color: 'var(--secondary)' }}>No execution history found.</div>;

    return (
        <div style={{ display: 'flex', flexDirection: 'column', gap: '0.5rem', padding: '0.5rem' }}>
            {runs.map(run => (
                <div 
                    key={run.id}
                    onClick={() => onSelectRun(run)}
                    style={{ 
                        display: 'flex', 
                        alignItems: 'center', 
                        justifyContent: 'space-between',
                        padding: '0.75rem 1rem', 
                        background: 'var(--card-bg)', 
                        border: '1px solid var(--border)',
                        borderRadius: '8px',
                        cursor: 'pointer',
                        transition: 'all 0.2s'
                    }}
                    onMouseOver={(e) => (e.currentTarget.style.borderColor = 'var(--accent)')}
                    onMouseOut={(e) => (e.currentTarget.style.borderColor = 'var(--border)')}
                >
                    <div style={{ display: 'flex', alignItems: 'center', gap: '1rem' }}>
                        {getStatusIcon(run.status)}
                        <div>
                            <div style={{ fontWeight: 600 }}>Run #{run.runNumber}</div>
                            <div style={{ fontSize: '0.75rem', color: 'var(--secondary)' }}>
                                {new Date(run.startTime).toLocaleString()}
                            </div>
                        </div>
                    </div>
                    <div style={{ fontSize: '0.85rem', fontWeight: 600, color: 'var(--secondary)' }}>
                        {run.endTime ? (
                            `${Math.round((new Date(run.endTime).getTime() - new Date(run.startTime).getTime()) / 1000)}s`
                        ) : '---'}
                    </div>
                </div>
            ))}
        </div>
    );
};

export default JobHistoryList;
