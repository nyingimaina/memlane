'use client';

import React from 'react';
import { ZestResponsiveLayout } from "jattac.libs.web.zest-responsive-layout";
import Sidebar from "@/components/Sidebar";
import ZestThemeProvider from "@/components/ZestThemeProvider";
import { UIProvider, useUI } from "@/logic/UIContext";
import GuidedTour from "@/components/GuidedTour";

import styles from "@/styles/theme.module.css";

interface ClientLayoutProps {
    children: React.ReactNode;
}

const LayoutContent: React.FC<ClientLayoutProps> = ({ children }) => {
    const { sidePane, closeSidePane, tutorialTriggered, clearTutorialTrigger } = useUI();

    return (
        <div className={styles.visibilityProvider}>
            <Sidebar />
            <GuidedTour manualTrigger={tutorialTriggered} onComplete={clearTutorialTrigger} />
            <ZestResponsiveLayout 
                sidePane={{
                    visible: sidePane.visible,
                    pane: sidePane.content,
                    title: sidePane.title,
                    widthRems: sidePane.widthRems,
                    onClose: closeSidePane
                }}
                detailPane={
                    <main className="main-content" style={{ paddingLeft: '80px', paddingTop: '20px' }}>
                        {children}
                    </main>
                }
            />
        </div>
    );
};

const ClientLayout: React.FC<ClientLayoutProps> = ({ children }) => {
    return (
        <ZestThemeProvider>
            <UIProvider>
                <LayoutContent>
                    {children}
                </LayoutContent>
            </UIProvider>
        </ZestThemeProvider>
    );
};

export default ClientLayout;
