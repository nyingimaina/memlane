'use client';

import React from 'react';
import { 
    FaSun, 
    FaCloudSun, 
    FaCloud, 
    FaCloudShowersHeavy, 
    FaCloudBolt,
    FaCircleXmark,
    FaCircleCheck
} from 'react-icons/fa6';
import { JobStatus } from '@/models/Job';

interface JobHealthIconProps {
    score: number; // 0 to 100
    total: number;
    success: number;
    lastRunStatus?: JobStatus;
}

const JobHealthIcon: React.FC<JobHealthIconProps> = ({ score, total, success, lastRunStatus }) => {
    
    const getHealthInfo = () => {
        if (score >= 80) return { icon: <FaSun color="#FFD700" />, label: 'Sunny', description: 'Very stable' };
        if (score >= 60) return { icon: <FaCloudSun color="#FFD700" />, label: 'Partly Cloudy', description: 'Stable' };
        if (score >= 40) return { icon: <FaCloud color="#A9A9A9" />, label: 'Cloudy', description: 'Unstable' };
        if (score >= 20) return { icon: <FaCloudShowersHeavy color="#4682B4" />, label: 'Rainy', description: 'Often fails' };
        return { icon: <FaCloudBolt color="#f72585" />, label: 'Stormy', description: 'Critical failures' };
    };

    const info = getHealthInfo();
    const tooltip = total > 0 
        ? `${info.label}: ${info.description} (${success}/${total} recent runs successful)`
        : 'No runs yet';

    const lastRunDisplay = lastRunStatus === JobStatus.Failed ? (
        <div style={{ display: 'flex', alignItems: 'center', gap: '0.25rem', color: '#f72585', fontWeight: 700, fontSize: '0.75rem' }}>
            <FaCircleXmark /> FAILED
        </div>
    ) : lastRunStatus === JobStatus.Completed ? (
        <div style={{ display: 'flex', alignItems: 'center', gap: '0.25rem', color: '#4cc9f0', fontWeight: 600, fontSize: '0.75rem' }}>
            <FaCircleCheck /> OK
        </div>
    ) : null;

    return (
        <div style={{ display: 'flex', flexDirection: 'column', gap: '0.25rem' }} title={tooltip}>
            <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
                <span style={{ fontSize: '1.2rem', display: 'flex' }}>{info.icon}</span>
                <span style={{ fontSize: '0.75rem', fontWeight: 600, color: 'var(--theme-text-light)' }}>
                    {total > 0 ? `${success}/${total}` : '-'}
                </span>
            </div>
            {lastRunDisplay}
        </div>
    );
};

export default JobHealthIcon;
