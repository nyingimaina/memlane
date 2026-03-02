'use client';

import React from 'react';
import { ZestResponsiveLayout } from "jattac.libs.web.zest-responsive-layout";
import Sidebar from "@/components/Sidebar";
import ZestThemeProvider from "@/components/ZestThemeProvider";

interface ClientLayoutProps {
    children: React.ReactNode;
}

const ClientLayout: React.FC<ClientLayoutProps> = ({ children }) => {
    return (
        <ZestThemeProvider>
            <ZestResponsiveLayout 
                sidePane={{
                    visible: true,
                    pane: <Sidebar />,
                    widthRems: 18
                }}
                detailPane={
                    <main className="main-content">
                        {children}
                    </main>
                }
            />
        </ZestThemeProvider>
    );
};

export default ClientLayout;
