import { useState } from 'react';
import { JobMetadata, BackupJobConfiguration } from '@/models/Job';
import { getTemplateForProvider } from '@/logic/ConnectionStringTemplates';

type JobCategory = 'Database' | 'Directory';

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
            compressionType: 'Zstandard',
            compressionOptionsJson: JSON.stringify({ level: 'Normal' })
        };

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

    const prepareSubmitData = () => {
        return {
            name,
            type: 'Backup',
            cronExpression: cronExpression || undefined,
            ignorePatterns: ignorePatterns || undefined,
            configurationJson: JSON.stringify(config)
        };
    };

    return {
        state: {
            name,
            cronExpression,
            ignorePatterns,
            config,
            jobCategory
        },
        actions: {
            setName,
            setCronExpression,
            setIgnorePatterns,
            setConfig,
            handleCategoryChange,
            handleProviderChange
        },
        prepareSubmitData
    };
};
