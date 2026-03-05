'use client';

import React from 'react';
import { 
    FaSun, 
    FaCloudSun, 
    FaCloud, 
    FaCloudShowersHeavy, 
    FaCloudBolt 
} from 'react-icons/fa6';

interface JobHealthIconProps {
    score: number; // 0 to 100
    total: number;
    success: number;
}

const JobHealthIcon: React.FC<JobHealthIconProps> = ({ score, total, success }) => {
    
    const getHealthInfo = () => {
        if (score >= 80) return { icon: <FaSun color="#FFD700" />, label: 'Sunny', description: 'Very stable' };
        if (score >= 60) return { icon: <FaCloudSun color="#FFD700" />, label: 'Partly Cloudy', description: 'Stable' };
        if (score >= 40) return { icon: <FaCloud color="#A9A9A9" />, label: 'Cloudy', description: 'Unstable' };
        if (score >= 20) return { icon: <FaCloudShowersHeavy color="#4682B4" />, label: 'Rainy', description: 'Often fails' };
        return { icon: <FaCloudBolt color="#708090" />, label: 'Stormy', description: 'Critical failures' };
    };

    const info = getHealthInfo();
    const tooltip = total > 0 
        ? `${info.label}: ${info.description} (${success}/${total} recent runs successful)`
        : 'No runs yet';

    return (
        <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }} title={tooltip}>
            <span style={{ fontSize: '1.2rem', display: 'flex' }}>{info.icon}</span>
        </div>
    );
};

export default JobHealthIcon;
