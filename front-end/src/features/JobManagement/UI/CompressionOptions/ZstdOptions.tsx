"use client";

import React from "react";
import styles from "../../Styles/CompressionOptions.module.css";

interface ZstdOptionsProps {
  optionsJson?: string;
  onChange: (optionsJson: string) => void;
}

const ZstdOptions: React.FC<ZstdOptionsProps> = ({ optionsJson, onChange }) => {
  const options = optionsJson ? JSON.parse(optionsJson) : { level: "Normal" };

  const handleLevelChange = (level: string) => {
    onChange(JSON.stringify({ ...options, level }));
  };

  return (
    <div className={styles.optionsContainer}>
      <label className={styles.label}>Zstd Level:</label>
      <select
        className={styles.select}
        value={options.level}
        onChange={(e) => handleLevelChange(e.target.value)}
      >
        <option value="Fastest">Fastest (Level 1)</option>
        <option value="Fast">Fast (Level 3)</option>
        <option value="Normal">Normal (Level 7)</option>
        <option value="Maximum">Maximum (Level 15)</option>
        <option value="Ultra">Ultra (Level 22)</option>
      </select>
    </div>
  );
};

export default ZstdOptions;
