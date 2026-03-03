'use client';

import React, { useState } from 'react';
import ZestTextbox from 'jattac.libs.web.zest-textbox';
import ZestButton from 'jattac.libs.web.zest-button';
import { JobMetadata, BackupJobConfiguration } from '@/models/Job';

interface JobFormProps {
    initialJob?: JobMetadata;
    onSubmit: (job: Partial<JobMetadata>) => Promise<void>;
    onCancel: () => void;
}

const JobForm: React.FC<JobFormProps> = ({ initialJob, onSubmit, onCancel }) => {
    const [name, setName] = useState(initialJob?.name || '');
    const [config, setConfig] = useState<BackupJobConfiguration>(
        initialJob?.configurationJson 
            ? JSON.parse(initialJob.configurationJson) 
            : { enableCompression: true, skipIfNoChanges: true, dbProvider: 'None', storageProvider: 'Folder' }
    );

    const handleSubmit = async () => {
        await onSubmit({
            name,
            type: 'Backup',
            configurationJson: JSON.stringify(config)
        });
    };

    const labelStyle: React.CSSProperties = { fontWeight: 600, fontSize: '0.9rem', color: 'var(--secondary)' };
    const inputGroupStyle: React.CSSProperties = { display: 'flex', flexDirection: 'column', gap: '0.5rem' };

    return (
        <div style={{ display: 'flex', flexDirection: 'column', gap: '1.5rem', padding: '1rem' }}>
            <div style={inputGroupStyle}>
                <label style={labelStyle}>Job Name</label>
                <ZestTextbox 
                    placeholder="e.g., Production SQL Server"
                    value={name}
                    zest={{
                        onTextChanged: (val) => setName(val || '')
                    }}
                />
            </div>

            <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
                <div style={inputGroupStyle}>
                    <label style={labelStyle}>Database Source</label>
                    <select 
                        value={config.dbProvider} 
                        onChange={(e) => setConfig({ ...config, dbProvider: e.target.value })}
                        style={{ padding: '0.75rem', borderRadius: '8px', border: '1px solid var(--border)', background: 'var(--card-bg)', color: 'var(--foreground)' }}
                    >
                        <option value="None">None (Files Only)</option>
                        <option value="SQL Server">SQL Server</option>
                        <option value="MariaDB">MariaDB</option>
                    </select>
                </div>

                <div style={inputGroupStyle}>
                    <label style={labelStyle}>Storage Destination</label>
                    <select 
                        value={config.storageProvider} 
                        onChange={(e) => setConfig({ ...config, storageProvider: e.target.value })}
                        style={{ padding: '0.75rem', borderRadius: '8px', border: '1px solid var(--border)', background: 'var(--card-bg)', color: 'var(--foreground)' }}
                    >
                        <option value="Folder">Local Folder (Testing)</option>
                        <option value="Local">Local Disk (Direct)</option>
                        <option value="S3">S3 Cloud</option>
                    </select>
                </div>
            </div>

            {config.dbProvider !== 'None' && (
                <div style={inputGroupStyle}>
                    <label style={labelStyle}>Database Connection String</label>
                    <ZestTextbox 
                        placeholder="Server=...;Database=...;"
                        value={config.dbConnectionString || ''}
                        zest={{
                            onTextChanged: (val) => setConfig({ ...config, dbConnectionString: val })
                        }}
                    />
                </div>
            )}

            <div style={inputGroupStyle}>
                <label style={labelStyle}>Source Folder (for local file sync)</label>
                <ZestTextbox 
                    placeholder="C:\Data\Files"
                    value={config.sourceDirectory || ''}
                    zest={{
                        onTextChanged: (val) => setConfig({ ...config, sourceDirectory: val })
                    }}
                />
            </div>

            <div style={inputGroupStyle}>
                <label style={labelStyle}>Target Destination Path</label>
                <ZestTextbox 
                    placeholder={config.storageProvider === 'S3' ? "my-bucket/backups" : "D:\Backups"}
                    value={config.targetDestination || ''}
                    zest={{
                        onTextChanged: (val) => setConfig({ ...config, targetDestination: val })
                    }}
                />
            </div>

            <div style={{ display: 'flex', gap: '2rem', padding: '0.5rem', background: 'var(--info-bg)', borderRadius: '8px' }}>
                <label style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', cursor: 'pointer' }}>
                    <input 
                        type="checkbox" 
                        checked={config.enableCompression} 
                        onChange={(e) => setConfig({ ...config, enableCompression: e.target.checked })}
                    />
                    <span style={labelStyle}>Enable Compression (ZIP)</span>
                </label>

                <label style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', cursor: 'pointer' }}>
                    <input 
                        type="checkbox" 
                        checked={config.skipIfNoChanges} 
                        onChange={(e) => setConfig({ ...config, skipIfNoChanges: e.target.checked })}
                    />
                    <span style={labelStyle}>Skip if no changes</span>
                </label>
            </div>

            {config.enableCompression && (
                <div style={inputGroupStyle}>
                    <label style={labelStyle}>Archive Filename</label>
                    <ZestTextbox 
                        placeholder="backup.zip"
                        value={config.archiveFileName || ''}
                        zest={{
                            onTextChanged: (val) => setConfig({ ...config, archiveFileName: val })
                        }}
                    />
                </div>
            )}

            <div style={{ display: 'flex', gap: '1rem', justifyContent: 'flex-end', marginTop: '1rem' }}>
                <ZestButton 
                    onClick={onCancel}
                    zest={{
                        visualOptions: { variant: 'standard' },
                        semanticType: 'cancel'
                    }}
                >
                    Cancel
                </ZestButton>
                <ZestButton 
                    onClick={handleSubmit}
                    zest={{
                        visualOptions: { variant: 'success' },
                        semanticType: 'save'
                    }}
                >
                    {initialJob ? 'Update Pipeline' : 'Create Pipeline'}
                </ZestButton>
            </div>
        </div>
    );
};

export default JobForm;
