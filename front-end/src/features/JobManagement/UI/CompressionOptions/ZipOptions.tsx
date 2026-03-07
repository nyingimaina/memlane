"use client";

import React from "react";
import styles from "../../Styles/CompressionOptions.module.css";

interface ZipOptionsProps {
  optionsJson?: string;
  onChange: (optionsJson: string) => void;
}

const ZipOptions: React.FC<ZipOptionsProps> = ({ optionsJson, onChange }) => {
  const options = optionsJson ? JSON.parse(optionsJson) : { level: "Optimal" };

  const handleLevelChange = (level: string) => {
    onChange(JSON.stringify({ ...options, level }));
  };

  return (
    <div className={styles.optionsContainer}>
      <label className={styles.label}>ZIP Level:</label>
      <select
        className={styles.select}
        value={options.level}
        onChange={(e) => handleLevelChange(e.target.value)}
      >
        <option value="Optimal">Optimal</option>
        <option value="Fastest">Fastest</option>
        <option value="None">No Compression</option>
      </select>
    </div>
  );
};

export default ZipOptions;
