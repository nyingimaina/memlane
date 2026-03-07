"use client";

import React from "react";
import styles from "../../Styles/CompressionOptions.module.css";

interface SevenZipOptionsProps {
  optionsJson?: string;
  onChange: (optionsJson: string) => void;
}

const SevenZipOptions: React.FC<SevenZipOptionsProps> = ({ optionsJson, onChange }) => {
  const options = optionsJson ? JSON.parse(optionsJson) : { level: "Normal" };

  const handleLevelChange = (level: string) => {
    onChange(JSON.stringify({ ...options, level }));
  };

  return (
    <div className={styles.optionsContainer}>
      <label className={styles.label}>7z Level:</label>
      <select
        className={styles.select}
        value={options.level}
        onChange={(e) => handleLevelChange(e.target.value)}
      >
        <option value="Fastest">Fastest</option>
        <option value="Fast">Fast</option>
        <option value="Normal">Normal</option>
        <option value="Maximum">Maximum</option>
        <option value="Ultra">Ultra</option>
      </select>
    </div>
  );
};

export default SevenZipOptions;
