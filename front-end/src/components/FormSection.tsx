'use client';

import React from 'react';

interface FormSectionProps {
    title?: string;
    children: React.ReactNode;
    className?: string;
    columns?: 1 | 2;
}

const FormSection: React.FC<FormSectionProps> = ({ title, children, className, columns = 1 }) => {
    const sectionStyle: React.CSSProperties = {
        display: 'flex',
        flexDirection: 'column',
        gap: '0.4rem',
        width: '100%'
    };

    const containerStyle: React.CSSProperties = {
        display: 'grid',
        gridTemplateColumns: columns === 2 ? '1fr 1fr' : '1fr',
        gap: '1rem',
        width: '100%'
    };

    const titleStyle: React.CSSProperties = {
        fontWeight: 600,
        fontSize: '0.85rem',
        color: 'var(--secondary)',
        marginBottom: '0.4rem'
    };

    return (
        <div style={sectionStyle} className={className}>
            {title && <label style={titleStyle}>{title}</label>}
            <div style={containerStyle}>
                {children}
            </div>
        </div>
    );
};

export default FormSection;
