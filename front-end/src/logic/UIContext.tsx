'use client';

import React, { createContext, useContext, useState, ReactNode } from 'react';

interface SidePaneConfig {
    visible: boolean;
    content: ReactNode;
    title?: ReactNode;
    widthRems?: number;
}

interface UIContextType {
    sidePane: SidePaneConfig;
    openSidePane: (content: ReactNode, title?: ReactNode, widthRems?: number) => void;
    closeSidePane: () => void;
}

const UIContext = createContext<UIContextType | undefined>(undefined);

export const UIProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
    const [sidePane, setSidePane] = useState<SidePaneConfig>({
        visible: false,
        content: null
    });

    const openSidePane = (content: ReactNode, title?: ReactNode, widthRems: number = 30) => {
        setSidePane({ visible: true, content, title, widthRems });
    };

    const closeSidePane = () => {
        setSidePane(prev => ({ ...prev, visible: false }));
    };

    return (
        <UIContext.Provider value={{ sidePane, openSidePane, closeSidePane }}>
            {children}
        </UIContext.Provider>
    );
};

export const useUI = () => {
    const context = useContext(UIContext);
    if (!context) {
        throw new Error('useUI must be used within a UIProvider');
    }
    return context;
};
