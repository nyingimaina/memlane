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

  return (
    <div className={styles.formContainer}>
      <div className={styles.header}>
        <TutorialIcon onClick={triggerTutorial} />
      </div>

      <FormSection title="Pipeline Name" className="job-name-field">
        <ZestTextbox
          placeholder="e.g., My Personal Documents"
          value={state.name}
          zest={{
            onTextChanged: (val) => actions.setName(val || ""),
            stretch: true,
          }}
        />
      </FormSection>

      <FormSection
        title="Source Directory"
        className="source-directory-field"
      >
        <DirectoryPicker
          placeholder="C:\Users\John\Documents"
          value={state.config.sourceDirectory || ""}
          onChange={(val) => actions.setConfig({ ...state.config, sourceDirectory: val })}
        />
      </FormSection>

      <FormSection
        title="Storage Destination"
        columns={2}
        className="storage-destination-config"
      >
        <div
          className="storage-provider-select"
          style={{ display: "flex", flexDirection: "column", gap: "0.4rem" }}
        >
          <select
            value={state.config.storageProvider}
            onChange={(e) =>
              actions.setConfig({ ...state.config, storageProvider: e.target.value })
            }
            style={{
              padding: "0.75rem",
              borderRadius: "8px",
              border: "1px solid var(--border)",
              background: "var(--card-bg)",
              color: "var(--foreground)",
              height: "100%",
            }}
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

      <div className={`${styles.checkboxGroup} ${theme.infoBg}`}>
        <label className={styles.checkboxLabel}>
          <input
            type="checkbox"
            checked={state.config.enableCompression}
            onChange={(e) =>
              actions.setConfig({ ...state.config, enableCompression: e.target.checked })
            }
          />
          <span style={{ color: "var(--secondary)" }}>Enable 7z Compression</span>
        </label>

        {state.config.enableCompression && (
          <select
            value={state.config.compressionLevel || "Normal"}
            onChange={(e) =>
              actions.setConfig({ ...state.config, compressionLevel: e.target.value as any })
            }
            style={{ 
              padding: "0.4rem", 
              borderRadius: "6px", 
              border: "1px solid var(--border)", 
              background: "var(--card-bg)", 
              color: "var(--foreground)",
              fontSize: "0.85rem"
            }}
          >
            <option value="Fastest">Fastest</option>
            <option value="Fast">Fast</option>
            <option value="Normal">Normal</option>
            <option value="Maximum">Maximum</option>
            <option value="Ultra">Ultra</option>
          </select>
        )}

        <label className={styles.checkboxLabel}>
          <input
            type="checkbox"
            checked={state.config.skipIfNoChanges}
            onChange={(e) =>
              actions.setConfig({ ...state.config, skipIfNoChanges: e.target.checked })
            }
          />
          <span style={{ color: "var(--secondary)" }}>Skip if no changes</span>
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
