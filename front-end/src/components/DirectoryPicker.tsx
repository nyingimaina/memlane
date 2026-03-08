"use client";
import styles from "./DirectoryPicker.module.css";

import React from "react";
import ZestTextbox from "jattac.libs.web.zest-textbox";
import { FaFolderOpen, FaFile } from "react-icons/fa6";

interface DirectoryPickerProps {
  value: string;
  onChange: (path: string) => void;
  placeholder?: string;
  label?: string;
  mode?: "directory" | "file";
}

const DirectoryPicker: React.FC<DirectoryPickerProps> = ({
  value,
  onChange,
  placeholder,
  label,
  mode = "directory",
}) => {
  const fileInputRef = React.useRef<HTMLInputElement>(null);

  const handleBrowse = async () => {
    try {
      if (mode === "directory" && "showDirectoryPicker" in window) {
        const handle = await (window as any).showDirectoryPicker();
        onChange(handle.name);
      } else if (mode === "file" && "showOpenFilePicker" in window) {
        const [handle] = await (window as any).showOpenFilePicker();
        onChange(handle.name);
      } else {
        // Fallback: Use hidden input
        fileInputRef.current?.click();
      }
    } catch (err) {
      console.error("Path selection cancelled or failed", err);
    }
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = e.target.files;
    if (files && files.length > 0) {
      const path = mode === "directory" 
        ? files[0].webkitRelativePath.split("/")[0] 
        : files[0].name;
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
        {...(mode === "directory" ? { 
            // @ts-ignore
            webkitdirectory: "", 
            directory: "" 
        } : {})}
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
          title={mode === "directory" ? "Browse folders..." : "Browse files..."}
          className={styles.browseButton}
        >
          {mode === "directory" ? <FaFolderOpen size={18} /> : <FaFile size={16} />}
        </button>
      </div>
    </div>
  );
};

export default DirectoryPicker;
