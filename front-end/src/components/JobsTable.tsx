'use client';

import React from 'react';
import ResponsiveTable, { IResponsiveTableColumnDefinition } from 'jattac.libs.web.responsive-table';
import OverflowMenu, { IOverflowMenuItem } from 'jattac.libs.web.overflow-menu';
import { JobMetadata, JobStatus } from '@/models/Job';
import { FaPlay, FaTrash, FaPenToSquare, FaCircle } from 'react-icons/fa6';

interface JobsTableProps {
    jobs: JobMetadata[];
    onTrigger: (id: number) => void;
    onEdit: (job: JobMetadata) => void;
    onDelete: (id: number) => void;
}

const JobsTable: React.FC<JobsTableProps> = ({ jobs, onTrigger, onEdit, onDelete }) => {
    
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
            columnId: 'name',
            displayLabel: 'Job Name',
            cellRenderer: (job) => (
                <div style={{ fontWeight: 600 }}>{job.name}</div>
            ),
            getSortableValue: (job) => job.name
        },
        {
            columnId: 'type',
            displayLabel: 'Type',
            cellRenderer: (job) => <span>{job.type}</span>,
            getSortableValue: (job) => job.type
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
                <span>{job.lastRunAt ? new Date(job.lastRunAt).toLocaleString() : 'Never'}</span>
            ),
            getSortableValue: (job) => job.lastRunAt || ''
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
