'use client';

import React, { useState, useEffect } from 'react';

interface CronBuilderProps {
    value: string;
    onChange: (cron: string) => void;
}

const commonSchedules = [
    { label: 'Manual Only', value: '' },
    { label: 'Every Minute', value: '* * * * *' },
    { label: 'Every Hour', value: '0 * * * *' },
    { label: 'Daily at Midnight', value: '0 0 * * *' },
    { label: 'Daily at 3 AM', value: '0 3 * * *' },
    { label: 'Weekly (Sunday)', value: '0 0 * * 0' },
    { label: 'Monthly (1st)', value: '0 0 1 * *' },
    { label: 'Custom...', value: 'custom' }
];

const CronBuilder: React.FC<CronBuilderProps> = ({ value, onChange }) => {
    const matched = commonSchedules.find(s => s.value === value && s.value !== 'custom');
    const [selection, setSelection] = useState(matched ? value : (value ? 'custom' : ''));
    const [customValue, setCustomValue] = useState(selection === 'custom' ? value : '');

    const handleSelectChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const val = e.target.value;
        setSelection(val);
        if (val !== 'custom') {
            onChange(val);
        } else {
            onChange(customValue || '* * * * *');
        }
    };

    const handleCustomChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const val = e.target.value;
        setCustomValue(val);
        onChange(val);
    };

    const selectStyle: React.CSSProperties = {
        padding: '0.75rem',
        borderRadius: '8px',
        border: '1px solid var(--border)',
        background: 'var(--card-bg)',
        color: 'var(--foreground)',
        width: '100%',
        cursor: 'pointer'
    };

    const inputStyle: React.CSSProperties = {
        padding: '0.75rem',
        borderRadius: '8px',
        border: '1px solid var(--border)',
        background: 'var(--card-bg)',
        color: 'var(--foreground)',
        width: '100%',
        marginTop: '0.5rem'
    };

    return (
        <div style={{ width: '100%' }}>
            <select value={selection} onChange={handleSelectChange} style={selectStyle}>
                {commonSchedules.map(s => (
                    <option key={s.label} value={s.value}>{s.label}</option>
                ))}
            </select>
            {selection === 'custom' && (
                <input 
                    type="text" 
                    value={customValue} 
                    onChange={handleCustomChange} 
                    placeholder="* * * * *" 
                    style={inputStyle}
                />
            )}
        </div>
    );
};

export default CronBuilder;
