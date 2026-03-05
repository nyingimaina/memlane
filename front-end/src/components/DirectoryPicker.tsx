'use client';

import React from 'react';
import ZestTextbox from 'jattac.libs.web.zest-textbox';
import { FaFolderOpen } from 'react-icons/fa6';

interface DirectoryPickerProps {
    value: string;
    onChange: (path: string) => void;
    placeholder?: string;
    label?: string;
}

const DirectoryPicker: React.FC<DirectoryPickerProps> = ({ value, onChange, placeholder, label }) => {
    const fileInputRef = React.useRef<HTMLInputElement>(null);
    
    const handleBrowse = async () => {
        try {
            if ('showDirectoryPicker' in window) {
                const handle = await (window as any).showDirectoryPicker();
                onChange(handle.name);
            } else {
                // Firefox/Safari Fallback: Use hidden input
                fileInputRef.current?.click();
            }
        } catch (err) {
            console.error("Directory selection cancelled or failed", err);
        }
    };

    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const files = e.target.files;
        if (files && files.length > 0) {
            // webkitRelativePath gives the path within the selected directory
            // We take the first part which is the directory name
            const path = files[0].webkitRelativePath.split('/')[0];
            onChange(path);
        }
    };

    const labelStyle: React.CSSProperties = { fontWeight: 600, fontSize: '0.85rem', color: 'var(--secondary)', marginBottom: '0.4rem' };

    return (
        <div style={{ display: 'flex', flexDirection: 'column', width: '100%' }}>
            {label && <label style={labelStyle}>{label}</label>}
            <input 
                type="file" 
                ref={fileInputRef} 
                style={{ display: 'none' }} 
                // @ts-ignore - webkitdirectory is non-standard but widely supported
                webkitdirectory="" 
                directory="" 
                onChange={handleFileChange}
            />
            <div style={{ display: 'flex', gap: '0.5rem', alignItems: 'center' }}>
                <div style={{ flex: 1 }}>
                    <ZestTextbox 
                        placeholder={placeholder}
                        value={value}
                        zest={{ 
                            onTextChanged: (val) => onChange(val || ''),
                            stretch: true
                        }}
                    />
                </div>
                <button 
                    onClick={handleBrowse}
                    title="Browse folders..."
                    style={{
                        padding: '0.75rem',
                        borderRadius: '8px',
                        border: '1px solid var(--border)',
                        background: 'var(--info-bg)',
                        color: 'var(--accent)',
                        cursor: 'pointer',
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center',
                        transition: 'all 0.2s'
                    }}
                    onMouseOver={(e) => (e.currentTarget.style.backgroundColor = 'var(--border)')}
                    onMouseOut={(e) => (e.currentTarget.style.backgroundColor = 'var(--info-bg)')}
                >
                    <FaFolderOpen size={18} />
                </button>
            </div>
        </div>
    );
};

export default DirectoryPicker;
