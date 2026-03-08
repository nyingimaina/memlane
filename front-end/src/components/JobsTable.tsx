'use client';

import React from 'react';
import ResponsiveTable, { IResponsiveTableColumnDefinition } from 'jattac.libs.web.responsive-table';
import OverflowMenu, { IOverflowMenuItem } from 'jattac.libs.web.overflow-menu';
import { JobMetadata, JobStatus } from '@/models/Job';
import JobHealthIcon from '@/components/JobHealthIcon';
import { FaPlay, FaTrash, FaPenToSquare, FaCircle, FaClockRotateLeft } from 'react-icons/fa6';

interface JobsTableProps {
    jobs: JobMetadata[];
    onTrigger: (id: number) => void;
    onEdit: (job: JobMetadata) => void;
    onDelete: (id: number) => void;
    onShowHistory: (job: JobMetadata) => void;
}

const JobsTable: React.FC<JobsTableProps> = ({ jobs, onTrigger, onEdit, onDelete, onShowHistory }) => {
    
    const getStatusColor = (status: JobStatus) => {
        switch (status) {
            case JobStatus.Completed: return 'var(--success)';
            case JobStatus.Failed: return 'var(--danger)';
            case JobStatus.InProgress: return 'var(--primary)';
            case JobStatus.Skipped: return 'var(--secondary)';
            default: return 'var(--warning)';
        }
    };

    const getStatusLabel = (status: JobStatus) => {
        return JobStatus[status];
    };

    const columns: IResponsiveTableColumnDefinition<JobMetadata>[] = [
        {
            columnId: 'health',
            displayLabel: 'Health',
            cellRenderer: (job) => <JobHealthIcon score={job.healthScore} total={job.totalRunsInWindow} success={job.successCountInWindow} lastRunStatus={job.lastRunStatus} />,
            getSortableValue: (job) => job.healthScore
        },
        {
            columnId: 'name',
            displayLabel: 'Job Name',
            cellRenderer: (job) => (
                <div style={{ fontWeight: 600 }}>{job.name}</div>
            ),
            getSortableValue: (job) => job.name
        },
        {
            columnId: 'status',
            displayLabel: 'Status',
            cellRenderer: (job) => (
                <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
                    <FaCircle 
                        className={job.status === JobStatus.InProgress ? 'status-active' : ''} 
                        style={{ color: getStatusColor(job.status), fontSize: '0.75rem' }} 
                    />
                    <span>{getStatusLabel(job.status)}</span>
                </div>
            ),
            getSortableValue: (job) => job.status
        },
        {
            columnId: 'lastRun',
            displayLabel: 'Last Run',
            cellRenderer: (job) => (
                <div style={{ display: 'flex', flexDirection: 'column' }}>
                    <span>{job.lastRunAt ? new Date(job.lastRunAt).toLocaleString() : 'Never'}</span>
                    {job.lastRunId && (
                        <span style={{ fontSize: '0.7rem', color: 'var(--secondary)', fontWeight: 600 }}>
                            #{job.lastRunId}
                        </span>
                    )}
                </div>
            ),
            getSortableValue: (job) => job.lastRunAt || ''
        },
        {
            columnId: 'nextRun',
            displayLabel: 'Next Run',
            cellRenderer: (job) => (
                <span style={{ color: 'var(--info)', fontWeight: 500 }}>
                    {job.nextRunAt ? new Date(job.nextRunAt).toLocaleString() : '---'}
                </span>
            ),
            getSortableValue: (job) => job.nextRunAt || ''
        },
        {
            columnId: 'actions',
            displayLabel: '',
            cellRenderer: (job) => {
                const menuItems: IOverflowMenuItem[] = [
                    {
                        content: (
                            <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
                                <FaPlay style={{ color: 'var(--success)' }} /> Run Now
                            </div>
                        ),
                        onClick: () => onTrigger(job.id),
                        enabled: job.status !== JobStatus.InProgress
                    },
                    {
                        content: (
                            <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
                                <FaClockRotateLeft style={{ color: 'var(--accent)' }} /> Execution History
                            </div>
                        ),
                        onClick: () => onShowHistory(job)
                    },
                    {
                        content: (
                            <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
                                <FaPenToSquare style={{ color: 'var(--primary)' }} /> Edit
                            </div>
                        ),
                        onClick: () => onEdit(job)
                    },
                    {
                        content: (
                            <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
                                <FaTrash style={{ color: 'var(--danger)' }} /> Delete
                            </div>
                        ),
                        onClick: () => onDelete(job.id)
                    }
                ];

                return <OverflowMenu items={menuItems} />;
            }
        }
    ];

    return (
        <div className="card" style={{ padding: '0' }}>
            <ResponsiveTable 
                columnDefinitions={columns}
                data={jobs}
                mobileBreakpoint={768}
                animationProps={{ animateOnLoad: true }}
            />
        </div>
    );
};

export default JobsTable;
