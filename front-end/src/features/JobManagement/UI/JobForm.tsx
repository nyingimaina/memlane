"use client";

import React from "react";
import ZestTextbox from "jattac.libs.web.zest-textbox";
import ZestButton from "jattac.libs.web.zest-button";
import { JobMetadata } from "@/models/Job";
import TutorialIcon from "@/components/TutorialIcon";
import CronBuilder from "@/components/CronBuilder";
import DirectoryPicker from "@/components/DirectoryPicker";
import FormSection from "@/components/FormSection";
import { useUI } from "@/logic/UIContext";
import { useJobForm } from "../Logic/useJobForm";

import ZipOptions from "./CompressionOptions/ZipOptions";
import ZstdOptions from "./CompressionOptions/ZstdOptions";

import styles from "../Styles/JobForm.module.css";
import theme from "../../../styles/theme.module.css";

interface JobFormProps {
  initialJob?: JobMetadata;
  onSubmit: (job: Partial<JobMetadata>) => Promise<void>;
  onCancel: () => void;
}

const JobForm: React.FC<JobFormProps> = ({
  initialJob,
  onSubmit,
  onCancel,
}) => {
  const { triggerTutorial } = useUI();
  const { state, actions, prepareSubmitData } = useJobForm(initialJob);

  const handleSubmit = async () => {
    const data = prepareSubmitData();
    await onSubmit(data);
  };

  const renderCompressionOptions = () => {
    const type = state.config.compressionType || "Zstandard";
    switch (type) {
      case "Zip":
        return <ZipOptions 
                  optionsJson={state.config.compressionOptionsJson} 
                  onChange={(json) => actions.setConfig({ ...state.config, compressionOptionsJson: json })} 
               />;
      case "Zstandard":
        return <ZstdOptions 
                  optionsJson={state.config.compressionOptionsJson} 
                  onChange={(json) => actions.setConfig({ ...state.config, compressionOptionsJson: json })} 
               />;
      default:
        return null;
    }
  };

  const categoryButtonStyle = (active: boolean): React.CSSProperties => ({
    flex: 1, 
    padding: '0.6rem', 
    border: 'none', 
    borderRadius: '10px',
    cursor: 'pointer',
    fontWeight: 600,
    backgroundColor: active ? '#ffffff' : 'transparent',
    color: active ? '#4361ee' : '#64748b',
    boxShadow: active ? '0 2px 4px rgba(0,0,0,0.1)' : 'none',
    transition: 'all 0.2s'
  });

  return (
    <div className={`${styles.formContainer} ${theme.visibilityProvider}`}>
      <div className={styles.header}>
        <TutorialIcon onClick={triggerTutorial} />
      </div>

      <div style={{ display: 'flex', background: '#f8fafc', borderRadius: '12px', padding: '0.25rem', marginBottom: '1rem' }}>
        <button 
            onClick={() => actions.handleCategoryChange('Database')}
            style={categoryButtonStyle(state.jobCategory === 'Database')}
        >
            Database Backup
        </button>
        <button 
            onClick={() => actions.handleCategoryChange('Directory')}
            style={categoryButtonStyle(state.jobCategory === 'Directory')}
        >
            Directory Backup
        </button>
      </div>

      <FormSection title="Pipeline Name" className="job-name-field">
        <ZestTextbox
          placeholder={state.jobCategory === 'Database' ? "e.g., Production SQL Server" : "e.g., My Personal Documents"}
          value={state.name}
          zest={{
            onTextChanged: (val) => actions.setName(val || ""),
            stretch: true,
          }}
        />
      </FormSection>

      {state.jobCategory === 'Database' && (
        <FormSection title="Database Source" columns={2}>
            <div className="database-provider-field">
                <select 
                    className={styles.storageProviderSelect}
                    value={state.config.dbProvider} 
                    onChange={(e) => actions.handleProviderChange(e.target.value)}
                >
                    <option value="SQL Server">SQL Server</option>
                    <option value="MariaDB">MariaDB</option>
                </select>
            </div>

            <div className="connection-string-field">
                <ZestTextbox 
                    placeholder="Server=...;Database=...;"
                    value={state.config.dbConnectionString || ''}
                    zest={{ 
                        onTextChanged: (val) => actions.setConfig({ ...state.config, dbConnectionString: val }),
                        stretch: true
                    }}
                />
            </div>

            <div className="sql-tool-path-field" style={{ gridColumn: 'span 2' }}>
                <ZestTextbox 
                    placeholder="Optional: Custom path to mariadb-dump / mysqldump / sqlcmd (e.g. C:\MariaDB\bin)"
                    value={state.config.sqlToolPath || ''}
                    zest={{ 
                        onTextChanged: (val) => actions.setConfig({ ...state.config, sqlToolPath: val }),
                        stretch: true
                    }}
                />
            </div>
        </FormSection>
      )}

      <FormSection
        title={state.jobCategory === 'Database' ? 'Source Directory (Optional Sync)' : 'Source Directory'} 
        className="source-directory-field"
      >
        <DirectoryPicker
          placeholder="C:\Data\Files"
          value={state.config.sourceDirectory || ""}
          onChange={(val) => actions.setConfig({ ...state.config, sourceDirectory: val })}
        />
      </FormSection>

      <FormSection
        title="Storage Destination"
        columns={2}
        className="storage-destination-config"
      >
        <div className="storage-provider-select">
          <select
            className={styles.storageProviderSelect}
            value={state.config.storageProvider}
            onChange={(e) =>
              actions.setConfig({ ...state.config, storageProvider: e.target.value })
            }
          >
            <option value="Folder">Local Folder (Testing)</option>
            <option value="Local">Local Disk (Direct)</option>
            <option value="S3">S3 Cloud</option>
          </select>
        </div>

        <div className="target-destination-field">
          <DirectoryPicker
            placeholder={
              state.config.storageProvider === "S3"
                ? "my-bucket/backups"
                : "D:\Backups"
            }
            value={state.config.targetDestination || ""}
            onChange={(val) => actions.setConfig({ ...state.config, targetDestination: val })}
          />
        </div>
      </FormSection>

      <FormSection title="Automation & Retention" columns={2}>
        <div className="cron-schedule-field">
          <CronBuilder value={state.cronExpression} onChange={actions.setCronExpression} />
        </div>

        {state.config.storageProvider !== "S3" && (
          <div className="rotation-field">
            <ZestTextbox
              type="number"
              value={state.config.retentionCount.toString()}
              zest={{
                onTextChanged: (val) =>
                  actions.setConfig({
                    ...state.config,
                    retentionCount: parseInt(val || "0"),
                  }),
                zSize: "md",
                stretch: true,
              }}
            />
          </div>
        )}
      </FormSection>

      <div className={styles.compressionSection}>
        <div className={styles.compressionHeader}>
            <label className={styles.checkboxLabel}>
            <input
                type="checkbox"
                checked={state.config.enableCompression}
                onChange={(e) =>
                actions.setConfig({ ...state.config, enableCompression: e.target.checked })
                }
            />
            <span>Enable Compression</span>
            </label>

            {state.config.enableCompression && (
            <select
                className={styles.typeSelect}
                value={state.config.compressionType || "Zstandard"}
                onChange={(e) =>
                actions.setConfig({ ...state.config, compressionType: e.target.value })
                }
            >
                <option value="Zstandard">High-Ratio Zstandard</option>
                <option value="Zip">Standard ZIP</option>
            </select>
            )}
        </div>

        {state.config.enableCompression && (
            <div className={styles.compressionOptions}>
                {renderCompressionOptions()}
            </div>
        )}
      </div>

      <div className={styles.checkboxGroup}>
        <label className={styles.checkboxLabel}>
          <input
            type="checkbox"
            checked={state.config.skipIfNoChanges}
            onChange={(e) =>
              actions.setConfig({ ...state.config, skipIfNoChanges: e.target.checked })
            }
          />
          <span>Skip if no changes</span>
        </label>
      </div>

      <FormSection
        title="Ignore Patterns (.memignore)"
        className="ignore-patterns-field"
      >
        <ZestTextbox
          placeholder="e.g., bin/&#10;obj/&#10;*.log&#10;node_modules/"
          value={state.ignorePatterns}
          zest={{
            onTextChanged: (val) => actions.setIgnorePatterns(val || ""),
            stretch: true,
          }}
        />
      </FormSection>

      <div className={styles.footer}>
        <ZestButton
          onClick={onCancel}
          zest={{
            visualOptions: { variant: "standard" },
            semanticType: "cancel",
          }}
        >
          Cancel
        </ZestButton>
        <ZestButton
          onClick={handleSubmit}
          zest={{ visualOptions: { variant: "success" }, semanticType: "save" }}
        >
          {initialJob ? "Update Pipeline" : "Create Pipeline"}
        </ZestButton>
      </div>
    </div>
  );
};

export default JobForm;
