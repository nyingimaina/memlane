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
            : { enableCompression: true, skipIfNoChanges: true }
    );

    const handleSubmit = async () => {
        await onSubmit({
            name,
            type: 'Backup',
            configurationJson: JSON.stringify(config)
        });
    };

    return (
        <div style={{ display: 'flex', flexDirection: 'column', gap: '1.5rem', padding: '1rem' }}>
            <div style={{ display: 'flex', flexDirection: 'column', gap: '0.5rem' }}>
                <label style={{ fontWeight: 600, fontSize: '0.9rem', color: 'var(--secondary)' }}>Job Name</label>
                <ZestTextbox 
                    placeholder="e.g., Production SQL Server"
                    value={name}
                    zest={{
                        onTextChanged: (val) => setName(val || '')
                    }}
                />
            </div>

            <div style={{ display: 'flex', flexDirection: 'column', gap: '0.5rem' }}>
                <label style={{ fontWeight: 600, fontSize: '0.9rem', color: 'var(--secondary)' }}>Source Directory</label>
                <ZestTextbox 
                    placeholder="C:\Data\Files"
                    value={config.sourceDirectory || ''}
                    zest={{
                        onTextChanged: (val) => setConfig({ ...config, sourceDirectory: val })
                    }}
                />
            </div>

            <div style={{ display: 'flex', flexDirection: 'column', gap: '0.5rem' }}>
                <label style={{ fontWeight: 600, fontSize: '0.9rem', color: 'var(--secondary)' }}>Target Directory</label>
                <ZestTextbox 
                    placeholder="D:\Backups"
                    value={config.targetDirectory || ''}
                    zest={{
                        onTextChanged: (val) => setConfig({ ...config, targetDirectory: val })
                    }}
                />
            </div>

            <div style={{ display: 'flex', gap: '1rem', justifyContent: 'flex-end', marginTop: '2rem' }}>
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
                    {initialJob ? 'Update Job' : 'Create Job'}
                </ZestButton>
            </div>
        </div>
    );
};

export default JobForm;
