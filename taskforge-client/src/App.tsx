import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthPortal } from './AuthPortal';
import { Workspace } from './Workspace';

export const App: React.FC = () => {
    return (
        <BrowserRouter>
            <Routes>
                {/* Auth Gate Routing */}
                <Route path="/login" element={<AuthPortal />} />

                {/* Base Workspace Context Redirection */}
                <Route
                    path="/workspace"
                    element={<Navigate to="/workspace/phoenix-redesign/kanban" replace />}
                />

                {/* Workspace Core Routes (Handles both background workspace and detail drawer state) */}
                <Route path="/workspace/:projectId/:viewType" element={<Workspace />} />
                <Route path="/workspace/:projectId/:viewType/task/:taskId" element={<Workspace />} />

                {/* Global Fallback Gate */}
                <Route path="*" element={<Navigate to="/login" replace />} />
            </Routes>
        </BrowserRouter>
    );
};