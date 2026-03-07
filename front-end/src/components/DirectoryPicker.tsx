"use client";
import styles from "./DirectoryPicker.module.css";

import React from "react";
import ZestTextbox from "jattac.libs.web.zest-textbox";
import { FaFolderOpen } from "react-icons/fa6";

interface DirectoryPickerProps {
  value: string;
  onChange: (path: string) => void;
  placeholder?: string;
  label?: string;
}

const DirectoryPicker: React.FC<DirectoryPickerProps> = ({
  value,
  onChange,
  placeholder,
  label,
}) => {
  const fileInputRef = React.useRef<HTMLInputElement>(null);

  const handleBrowse = async () => {
    try {
      if ("showDirectoryPicker" in window) {
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
      const path = files[0].webkitRelativePath.split("/")[0];
      onChange(path);
    }
  };

  return (
    <div className={styles.mainContainer}>
      {label && <label className={styles.label}>{label}</label>}
      <input
        type="file"
        ref={fileInputRef}
        style={{ display: "none" }}
        // @ts-ignore
        webkitdirectory=""
        directory=""
        onChange={handleFileChange}
      />
      <div className={styles.container}>
        <div className={styles.textboxWrapper}>
          <ZestTextbox
            placeholder={placeholder}
            value={value}
            zest={{
              onTextChanged: (val) => onChange(val || ""),
              stretch: true,
            }}
          />
        </div>
        <button
          onClick={handleBrowse}
          title="Browse folders..."
          className={styles.browseButton}
        >
          <FaFolderOpen size={18} />
        </button>
      </div>
    </div>
  );
};

export default DirectoryPicker;
