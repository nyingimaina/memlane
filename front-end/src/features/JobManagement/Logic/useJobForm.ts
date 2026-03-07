import { useState } from 'react';
import { JobMetadata, BackupJobConfiguration } from '@/models/Job';

export const useJobForm = (initialJob?: JobMetadata) => {
    const [name, setName] = useState(initialJob?.name || '');
    const [cronExpression, setCronExpression] = useState(initialJob?.cronExpression || '');
    const [ignorePatterns, setIgnorePatterns] = useState(initialJob?.ignorePatterns || '');
    
    const initialConfig: BackupJobConfiguration = initialJob?.configurationJson 
        ? JSON.parse(initialJob.configurationJson) 
        : { 
            enableCompression: true, 
            skipIfNoChanges: true, 
            dbProvider: 'None', 
            storageProvider: 'Folder', 
            retentionCount: 5,
            compressionType: 'Zip',
            compressionOptionsJson: JSON.stringify({ level: 'Optimal' })
        };

    const [config, setConfig] = useState<BackupJobConfiguration>(initialConfig);

    const prepareSubmitData = () => {
        return {
            name,
            type: 'Backup',
            cronExpression: cronExpression || undefined,
            ignorePatterns: ignorePatterns || undefined,
            configurationJson: JSON.stringify({
                ...config,
                dbProvider: 'None', // Enforced for v1.0.0
                dbConnectionString: undefined
            })
        };
    };

    return {
        state: {
            name,
            cronExpression,
            ignorePatterns,
            config
        },
        actions: {
            setName,
            setCronExpression,
            setIgnorePatterns,
            setConfig
        },
        prepareSubmitData
    };
};
