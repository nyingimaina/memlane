'use client';

import React from 'react';
import { HiOutlineQuestionMarkCircle } from 'react-icons/hi2';

interface TutorialIconProps {
    onClick: () => void;
    title?: string;
}

const TutorialIcon: React.FC<TutorialIconProps> = ({ onClick, title = 'Launch Tutorial' }) => {
    return (
        <div 
            onClick={onClick}
            title={title}
            style={{
                cursor: 'pointer',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                padding: '0.5rem',
                borderRadius: '50%',
                transition: 'background-color 0.2s',
                color: 'var(--accent)',
            }}
            onMouseOver={(e) => (e.currentTarget.style.backgroundColor = 'var(--info-bg)')}
            onMouseOut={(e) => (e.currentTarget.style.backgroundColor = 'transparent')}
        >
            <HiOutlineQuestionMarkCircle size={24} />
        </div>
    );
};

export default TutorialIcon;
