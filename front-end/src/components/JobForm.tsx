'use client';

import React, { useState } from 'react';
import ZestTextbox from 'jattac.libs.web.zest-textbox';
import ZestButton from 'jattac.libs.web.zest-button';
import { JobMetadata, BackupJobConfiguration } from '@/models/Job';
import TutorialIcon from '@/components/TutorialIcon';
import CronBuilder from '@/components/CronBuilder';
import DirectoryPicker from '@/components/DirectoryPicker';
import FormSection from '@/components/FormSection';
import { useUI } from '@/logic/UIContext';
import { getTemplateForProvider } from '@/logic/ConnectionStringTemplates';

interface JobFormProps {
    initialJob?: JobMetadata;
    onSubmit: (job: Partial<JobMetadata>) => Promise<void>;
    onCancel: () => void;
}

type JobCategory = 'Database' | 'Directory';

const JobForm: React.FC<JobFormProps> = ({ initialJob, onSubmit, onCancel }) => {
    const { triggerTutorial } = useUI();
    const [name, setName] = useState(initialJob?.name || '');
    const [cronExpression, setCronExpression] = useState(initialJob?.cronExpression || '');
    const [ignorePatterns, setIgnorePatterns] = useState(initialJob?.ignorePatterns || '');
    
    const initialConfig: BackupJobConfiguration = initialJob?.configurationJson 
        ? JSON.parse(initialJob.configurationJson) 
        : { enableCompression: true, skipIfNoChanges: true, dbProvider: 'None', storageProvider: 'Folder', retentionCount: 5 };

    const [config, setConfig] = useState<BackupJobConfiguration>(initialConfig);
    const [jobCategory, setJobCategory] = useState<JobCategory>(
        initialConfig.dbProvider && initialConfig.dbProvider !== 'None' ? 'Database' : 'Directory'
    );

    const handleCategoryChange = (category: JobCategory) => {
        setJobCategory(category);
        if (category === 'Directory') {
            setConfig({ ...config, dbProvider: 'None', dbConnectionString: undefined });
        } else {
            const defaultProvider = config.dbProvider === 'None' ? 'SQL Server' : config.dbProvider;
            setConfig({ 
                ...config, 
                dbProvider: defaultProvider,
                dbConnectionString: config.dbConnectionString || getTemplateForProvider(defaultProvider || 'SQL Server')
            });
        }
    };

    const handleProviderChange = (provider: string) => {
        const template = getTemplateForProvider(provider);
        setConfig({ ...config, dbProvider: provider, dbConnectionString: template });
    };

    const handleSubmit = async () => {
        await onSubmit({
            name,
            type: 'Backup',
            cronExpression: cronExpression || undefined,
            ignorePatterns: ignorePatterns || undefined,
            configurationJson: JSON.stringify(config)
        });
    };

    const categoryButtonStyle = (active: boolean): React.CSSProperties => ({
        flex: 1, 
        padding: '0.6rem', 
        border: 'none', 
        borderRadius: '10px',
        cursor: 'pointer',
        fontWeight: 600,
        backgroundColor: active ? 'var(--card-bg)' : 'transparent',
        color: active ? 'var(--accent)' : 'var(--secondary)',
        boxShadow: active ? '0 2px 4px rgba(0,0,0,0.1)' : 'none',
        transition: 'all 0.2s'
    });

    return (
        <div style={{ display: 'flex', flexDirection: 'column', gap: '1.5rem', padding: '1rem' }} className="job-form-container">
            <div style={{ display: 'flex', justifyContent: 'flex-end' }}>
                <TutorialIcon onClick={triggerTutorial} />
            </div>

            <div style={{ display: 'flex', background: 'var(--info-bg)', borderRadius: '12px', padding: '0.25rem' }}>
                <button 
                    onClick={() => handleCategoryChange('Database')}
                    style={categoryButtonStyle(jobCategory === 'Database')}
                >
                    Database Backup
                </button>
                <button 
                    onClick={() => handleCategoryChange('Directory')}
                    style={categoryButtonStyle(jobCategory === 'Directory')}
                >
                    Directory Backup
                </button>
            </div>

            <FormSection title="Pipeline Name" className="job-name-field">
                <ZestTextbox 
                    placeholder="e.g., Production SQL Server"
                    value={name}
                    zest={{ 
                        onTextChanged: (val) => setName(val || ''),
                        stretch: true
                    }}
                />
            </FormSection>

            {jobCategory === 'Database' && (
                <FormSection title="Database Source" columns={2}>
                    <div className="database-provider-field" style={{ display: 'flex', flexDirection: 'column', gap: '0.4rem' }}>
                        <select 
                            value={config.dbProvider} 
                            onChange={(e) => handleProviderChange(e.target.value)}
                            style={{ padding: '0.75rem', borderRadius: '8px', border: '1px solid var(--border)', background: 'var(--card-bg)', color: 'var(--foreground)', height: '100%' }}
                        >
                            <option value="SQL Server">SQL Server</option>
                            <option value="MariaDB">MariaDB</option>
                        </select>
                    </div>

                    <div className="connection-string-field">
                        <ZestTextbox 
                            placeholder="Server=...;Database=...;"
                            value={config.dbConnectionString || ''}
                            zest={{ 
                                onTextChanged: (val) => setConfig({ ...config, dbConnectionString: val }),
                                stretch: true
                            }}
                        />
                    </div>
                </FormSection>
            )}

            <FormSection 
                title={jobCategory === 'Database' ? 'Source Directory (Optional Sync)' : 'Source Directory'} 
                className="source-directory-field"
            >
                <DirectoryPicker 
                    placeholder="C:\Data\Files"
                    value={config.sourceDirectory || ''}
                    onChange={(val) => setConfig({ ...config, sourceDirectory: val })}
                />
            </FormSection>

            <FormSection title="Storage Destination" columns={2} className="storage-destination-config">
                <div className="storage-provider-select" style={{ display: 'flex', flexDirection: 'column', gap: '0.4rem' }}>
                    <select 
                        value={config.storageProvider} 
                        onChange={(e) => setConfig({ ...config, storageProvider: e.target.value })}
                        style={{ padding: '0.75rem', borderRadius: '8px', border: '1px solid var(--border)', background: 'var(--card-bg)', color: 'var(--foreground)', height: '100%' }}
                    >
                        <option value="Folder">Local Folder (Testing)</option>
                        <option value="Local">Local Disk (Direct)</option>
                        <option value="S3">S3 Cloud</option>
                    </select>
                </div>

                <div className="target-destination-field">
                    <DirectoryPicker 
                        placeholder={config.storageProvider === 'S3' ? "my-bucket/backups" : "D:\Backups"}
                        value={config.targetDestination || ''}
                        onChange={(val) => setConfig({ ...config, targetDestination: val })}
                    />
                </div>
            </FormSection>

            <FormSection title="Automation & Retention" columns={2}>
                <div className="cron-schedule-field">
                    <CronBuilder value={cronExpression} onChange={setCronExpression} />
                </div>

                {config.storageProvider !== 'S3' && (
                    <div className="rotation-field">
                        <ZestTextbox 
                            type="number"
                            value={config.retentionCount.toString()}
                            zest={{ 
                                onTextChanged: (val) => setConfig({ ...config, retentionCount: parseInt(val || '0') }),
                                zSize: 'md',
                                stretch: true
                            }}
                        />
                    </div>
                )}
            </FormSection>

            <div style={{ display: 'flex', gap: '2rem', padding: '0.5rem', background: 'var(--info-bg)', borderRadius: '8px' }}>
                <label style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', cursor: 'pointer' }}>
                    <input 
                        type="checkbox" 
                        checked={config.enableCompression} 
                        onChange={(e) => setConfig({ ...config, enableCompression: e.target.checked })}
                    />
                    <span style={{ fontWeight: 600, fontSize: '0.85rem', color: 'var(--secondary)' }}>Enable ZIP Compression</span>
                </label>

                <label style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', cursor: 'pointer' }}>
                    <input 
                        type="checkbox" 
                        checked={config.skipIfNoChanges} 
                        onChange={(e) => setConfig({ ...config, skipIfNoChanges: e.target.checked })}
                    />
                    <span style={{ fontWeight: 600, fontSize: '0.85rem', color: 'var(--secondary)' }}>Skip if no changes</span>
                </label>
            </div>

            <FormSection title="Ignore Patterns (.memignore)" className="ignore-patterns-field">
                <ZestTextbox 
                    placeholder="e.g., bin/&#10;obj/&#10;*.log&#10;node_modules/"
                    value={ignorePatterns}
                    zest={{ 
                        onTextChanged: (val) => setIgnorePatterns(val || ''),
                        stretch: true,
                        // Note: ZestTextbox might not natively support textarea without a specific flag, 
                        // but if it's the corporate Zest textbox it usually handles multiline if it's large.
                        // For now using default.
                    }}
                />
            </FormSection>

            <div style={{ display: 'flex', gap: '1rem', justifyContent: 'flex-end', marginTop: '1rem' }}>
                <ZestButton 
                    onClick={onCancel}
                    zest={{ visualOptions: { variant: 'standard' }, semanticType: 'cancel' }}
                >
                    Cancel
                </ZestButton>
                <ZestButton 
                    onClick={handleSubmit}
                    zest={{ visualOptions: { variant: 'success' }, semanticType: 'save' }}
                >
                    {initialJob ? 'Update Pipeline' : 'Create Pipeline'}
                </ZestButton>
            </div>
        </div>
    );
};

export default JobForm;
