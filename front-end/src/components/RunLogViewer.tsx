'use client';

import React, { useEffect, useRef } from 'react';
import { JobRun, JobStatus } from '@/models/Job';

interface RunLogViewerProps {
    run: JobRun;
    isLive?: boolean;
}

const RunLogViewer: React.FC<RunLogViewerProps> = ({ run, isLive }) => {
    const logRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        if (isLive && logRef.current) {
            logRef.current.scrollTop = logRef.current.scrollHeight;
        }
    }, [run.logs, isLive]);

    const getStatusColor = (status: JobStatus) => {
        switch (status) {
            case JobStatus.Completed: return 'var(--success)';
            case JobStatus.Failed: return 'var(--danger)';
            case JobStatus.InProgress: return 'var(--info)';
            case JobStatus.Skipped: return 'var(--secondary)';
            default: return 'var(--foreground)';
        }
    };

    return (
        <div style={{ display: 'flex', flexDirection: 'column', height: '100%', gap: '1rem' }}>
            <div style={{ padding: '1rem', background: 'var(--info-bg)', borderRadius: '8px', borderLeft: `4px solid ${getStatusColor(run.status)}` }}>
                <div style={{ fontWeight: 700, fontSize: '1.1rem' }}>
                    Run #{run.id} - {JobStatus[run.status]}
                </div>
                <div style={{ fontSize: '0.85rem', color: 'var(--secondary)', marginTop: '0.25rem' }}>
                    Started: {new Date(run.startTime).toLocaleString()} 
                    {run.endTime && ` | Ended: ${new Date(run.endTime).toLocaleString()}`}
                </div>
                {run.resultMessage && (
                    <div style={{ marginTop: '0.5rem', fontWeight: 600, color: run.status === JobStatus.Failed ? 'var(--danger)' : 'inherit' }}>
                        {run.resultMessage}
                    </div>
                )}
            </div>

            <div 
                ref={logRef}
                style={{ 
                    flex: 1, 
                    background: '#1e1e1e', 
                    color: '#d4d4d4', 
                    padding: '1.5rem', 
                    borderRadius: '8px', 
                    fontFamily: 'monospace', 
                    fontSize: '0.9rem', 
                    lineHeight: '1.5',
                    overflowY: 'auto',
                    whiteSpace: 'pre-wrap',
                    boxShadow: 'inset 0 2px 10px rgba(0,0,0,0.5)'
                }}
            >
                {run.logs || 'Initializing console...'}
                {isLive && run.status === JobStatus.InProgress && (
                    <span className="status-pulse" style={{ display: 'inline-block', width: '8px', height: '15px', background: 'var(--info)', marginLeft: '4px', verticalAlign: 'middle' }} />
                )}
            </div>
        </div>
    );
};

export default RunLogViewer;
