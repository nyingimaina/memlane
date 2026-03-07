'use client';

import React from 'react';
import styles from '@/styles/FormSection.module.css';

interface FormSectionProps {
    title?: string;
    children: React.ReactNode;
    className?: string;
    columns?: 1 | 2;
}

const FormSection: React.FC<FormSectionProps> = ({ title, children, className, columns = 1 }) => {
    return (
        <div className={`${styles.section} ${className || ''}`}>
            {title && <label className={styles.title}>{title}</label>}
            <div className={columns === 2 ? styles.container2 : styles.container1}>
                {children}
            </div>
        </div>
    );
};

export default FormSection;
