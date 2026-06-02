import React, { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import type { Task, FilterType } from './types';
import { DashboardBoard } from './DashboardBoard';
import { DashboardTable } from './DashboardTable';
import { TaskDetail } from './TaskDetail';

const INITIAL_TASKS: Task[] = [
    {
        id: 'task-1',
        code: 'TF-101',
        title: 'DB Schema Optimization',
        desc: 'Optimize structural indexing thresholds for high-volume logs database layers. Currently, query metrics throw high latency latency chains.',
        priority: 'High',
        status: 'To Do',
        isMine: true,
        project: 'Phoenix Redesign',
        projectType: 'Internal Tech',
        assignee: 'Alex Mercer',
        startDate: '2026-06-01',
        dueDate: '2026-06-15',
        loggedTime: 6,
        estimatedTime: 12,
        subtasks: [
            { id: 'st-1', text: 'Profile slow logging clusters using EXPLAIN ANALYZE', done: true },
            { id: 'st-2', text: 'Construct staging benchmark simulation indexes', done: false }
        ],
        comments: [
            { id: 'c-1', author: 'Tom Miller', avatar: 'TM', text: 'Verify production locking index parameters prior to final deployment sequence.', timestamp: 'Yesterday' }
        ]
    },
    {
        id: 'task-2',
        code: 'TF-102',
        title: 'Localization Keys',
        desc: 'Inject global multi-language translation localization vectors down across user workspace profiles.',
        priority: 'Low',
        status: 'To Do',
        isMine: false,
        project: 'Nova Mobile App',
        projectType: 'Acme Corp',
        assignee: 'Sarah Jenkins',
        startDate: '2026-06-02',
        dueDate: '2026-06-20',
        loggedTime: 2,
        estimatedTime: 8,
        subtasks: [],
        comments: []
    }
];

export const Workspace: React.FC = () => {
    const navigate = useNavigate();

    const { projectId, viewType, taskId } = useParams<{
        projectId: string;
        viewType: string;
        taskId: string;
    }>();

    const activeProjectSlug = projectId || 'phoenix-redesign';
    const activeView = viewType === 'table' ? 'table' : 'kanban';
    const selectedTaskId = taskId || null;

    const [sidebarOpen, setSidebarOpen] = useState<boolean>(true);
    const [projectsExpanded, setProjectsExpanded] = useState<boolean>(true);
    const [filterMenuOpen, setFilterMenuOpen] = useState<boolean>(false);
    const [activeFilter, setActiveFilter] = useState<FilterType>('all');
    //const [activeView, setActiveView] = useState<ViewType>('kanban');
    const [activeProject] = useState<string>('Phoenix Redesign');

    const [tasks, setTasks] = useState<Task[]>(INITIAL_TASKS);
    //const [selectedTaskId, setSelectedTaskId] = useState<string | null>(null);
    const currentTask = tasks.find((t) => t.id === selectedTaskId) || null;
    const projectFilteredTasks = tasks.filter((t) => {
        const slug = t.project.toLowerCase().replace(/\s+/g, '-');
        return slug === activeProjectSlug;
    });

    // ==========================================
    // NAVIGATION HANDLERS (URL Mutators)
    // ==========================================
    const handleProjectSwitch = (projectName: string) => {
        const slug = projectName.toLowerCase().replace(/\s+/g, '-');
        navigate(`/workspace/${slug}/${activeView}`);
    };

    const handleViewToggle = (targetView: 'kanban' | 'table') => {
        navigate(`/workspace/${activeProjectSlug}/${targetView}`);
    };

    const handleSelectTask = (id: string) => {
        navigate(`/workspace/${activeProjectSlug}/${activeView}/task/${id}`);
    };

    const handleCloseDetailPane = () => {
        navigate(`/workspace/${activeProjectSlug}/${activeView}`);
    };

    const handleLogout = () => {
        navigate('/login');
    };

    const updateTaskField = <K extends keyof Task>(id: string, field: K, value: Task[K]) => {
        setTasks((prev) => prev.map((t) => (t.id === id ? { ...t, [field]: value } : t)));
    };

    return (
        <div className="h-screen w-screen bg-slate-50 flex overflow-hidden antialiased text-slate-800 selection:bg-emerald-500 selection:text-white font-['Inter',_sans-serif]">
            {/* --- RENDER: CONSISTENT SIDEBAR VIEW --- */}
            <aside className={`bg-slate-900 text-slate-400 flex flex-col justify-between border-r border-slate-800 shrink-0 z-30 transition-all duration-300 ${sidebarOpen ? 'w-64 p-5' : 'w-0 p-0 border-r-0 overflow-hidden'}`}>
                <div className="space-y-6">
                    <div className="flex items-center justify-between">
                        <div className="flex items-center gap-3">
                            <div className="h-8 w-8 bg-emerald-600 rounded-lg flex items-center justify-center text-white font-black text-sm">TF</div>
                            <span className="text-white font-bold text-base whitespace-nowrap">TaskForge</span>
                        </div>
                        <button onClick={() => setSidebarOpen(false)} className="text-slate-500 hover:text-slate-200 p-1 rounded-lg hover:bg-slate-800">
                            <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2.5"><path strokeLinecap="round" strokeLinejoin="round" d="M11 19l-7-7 7-7M20 19l-7-7 7-7" /></svg>
                        </button>
                    </div>

                    <div className="space-y-2">
                        <label className="text-[10px] font-bold uppercase text-slate-500 tracking-wider px-2">Workspace Filter</label>
                        <div className="relative">
                            <button onClick={() => setFilterMenuOpen(!filterMenuOpen)} className="w-full flex items-center justify-between bg-slate-800 border border-slate-700 text-slate-200 px-3 py-2.5 rounded-xl text-xs font-semibold">
                                <span className="truncate">{activeFilter === 'all' ? 'All Workspace' : 'My Tasks Only'}</span>
                                <svg className="h-3 w-3 text-slate-500 ml-1" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2.5"><path strokeLinecap="round" strokeLinejoin="round" d="M19 9l-7 7-7-7" /></svg>
                            </button>
                            {filterMenuOpen && (
                                <>
                                    <div className="fixed inset-0 z-40" onClick={() => setFilterMenuOpen(false)} />
                                    <div className="absolute left-0 right-0 mt-2 bg-slate-800 border border-slate-700 rounded-xl shadow-2xl z-50 py-1 text-xs">
                                        <button onClick={() => { setActiveFilter('all'); setFilterMenuOpen(false); }} className="w-full text-left px-3 py-2.5 hover:bg-slate-700/60">All Workspace</button>
                                        <button onClick={() => { setActiveFilter('mine'); setFilterMenuOpen(false); }} className="w-full text-left px-3 py-2.5 hover:bg-slate-700/60">My Tasks Only</button>
                                    </div>
                                </>
                            )}
                        </div>
                    </div>

                    <div className="space-y-2 pt-2">
                        <button onClick={() => setProjectsExpanded(!projectsExpanded)} className="w-full flex items-center justify-between text-[10px] font-bold uppercase text-slate-500 tracking-wider px-2 hover:text-slate-300">
                            <span>Projects</span>
                            <svg className={`h-3 w-3 text-slate-500 transform transition-transform ${projectsExpanded ? 'rotate-90' : 'rotate-0'}`} fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2.5"><path strokeLinecap="round" strokeLinejoin="round" d="M9 5l7 7-7 7" /></svg>
                        </button>
                        <div className={`space-y-1 overflow-hidden transition-all duration-300 ${projectsExpanded ? 'max-h-40' : 'max-h-0'}`}>
                            {['Phoenix Redesign', 'Nova Mobile App'].map((projectName) => {
                                // Convert "Phoenix Redesign" to "phoenix-redesign"
                                const projectSlug = projectName.toLowerCase().replace(/\s+/g, '-');
                                const isActive = activeProjectSlug === projectSlug;

                                return (
                                    <button
                                        key={projectSlug}
                                        onClick={() => handleProjectSwitch(projectName)}
                                        className={`w-full text-left px-3 py-2 rounded-xl text-sm font-medium transition-colors duration-200 ${isActive
                                                ? 'bg-slate-800 text-white'
                                                : 'hover:bg-slate-800/40 text-slate-400'
                                            }`}
                                    >
                                        {projectName}
                                    </button>
                                );
                            })}
                            <button onClick={handleLogout} className="mt-auto">Logout</button>
                        </div>
                    </div>
                </div>
            </aside>

            {/* --- RIGHT SIDE LAYOUT BASE --- */}
            <div className="flex-1 flex flex-col overflow-hidden relative bg-slate-50">

                {/* --- RENDER: CONSISTENT NAVIGATION HEADER --- */}
                <header className="h-16 bg-white border-b border-slate-200 flex items-center justify-between px-6 md:px-8 shrink-0 z-20">
                    <div className="flex items-center gap-3 overflow-x-auto py-1">
                        {!sidebarOpen && (
                            <button onClick={() => setSidebarOpen(true)} className="p-2 bg-slate-100 hover:bg-slate-200 text-slate-700 rounded-xl">
                                <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2.5"><path strokeLinecap="round" strokeLinejoin="round" d="M4 6h16M4 12h16M4 18h16" /></svg>
                            </button>
                        )}
                        <div className="flex items-center gap-2 text-xs font-semibold text-slate-400">
                            <span>{activeProject}</span>
                            <svg className="h-2.5 w-2.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2.5"><path strokeLinecap="round" strokeLinejoin="round" d="M9 5l7 7-7 7" /></svg>
                            <span>Sprint 24 Q3</span>
                        </div>
                    </div>

                    <div className="flex items-center gap-3">
                        <div className={`bg-slate-100 p-1 rounded-xl flex gap-1 border border-slate-200 text-xs font-bold ${currentTask ? 'hidden' : ''}`}>
                            <button
                                id="view-kanban-btn"
                                onClick={() => handleViewToggle('kanban')}
                                className={`px-3 py-1.5 rounded-lg transition flex items-center gap-1.5 ${activeView === 'kanban'
                                        ? 'bg-white text-slate-900 shadow-sm'
                                        : 'text-slate-500 hover:text-slate-800'
                                    }`}
                            >
                                <svg className="h-3.5 w-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2">
                                    <path strokeLinecap="round" strokeLinejoin="round" d="M9 17V7m0 10a2 2 0 01-2 2H5a2 2 0 01-2-2V7a2 2 0 012-2h2a2 2 0 012 2m0 10a2 2 0 002 2h2a2 2 0 002-2V7a2 2 0 00-2-2h-2a2 2 0 00-2 2" />
                                </svg>
                                Board
                            </button>
                            <button
                                id="view-table-btn"
                                onClick={() => handleViewToggle('table')}
                                className={`px-3 py-1.5 rounded-lg transition flex items-center gap-1.5 ${activeView === 'table'
                                        ? 'bg-white text-slate-900 shadow-sm'
                                        : 'text-slate-500 hover:text-slate-800'
                                    }`}
                            >
                                <svg className="h-3.5 w-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2">
                                    <path strokeLinecap="round" strokeLinejoin="round" d="M3 10h18M3 14h18m-9-4v8m-7 0h14a2 2 0 002-2V6a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                                </svg>
                                Table View
                            </button>

                        </div>
                        {currentTask && (
                            <button onClick={handleCloseDetailPane} className="px-3 py-1.5 bg-slate-100 hover:bg-slate-200 text-slate-700 text-xs font-bold rounded-xl">
                                Return to Workspace
                            </button>
                        )}
                    </div>
                </header>

                {/* --- MAIN DISPLAY CONTROLLER VIEWPORT --- */}
                <div className="flex-1 relative overflow-hidden">

                    {/* Dashboard Hub View Wrapper */}
                    <div className={`absolute inset-0 p-6 md:p-8 space-y-4 overflow-y-auto transition-opacity duration-200 ${currentTask ? 'opacity-0 pointer-events-none' : 'opacity-100'}`}>
                        {activeView === 'kanban' ? (
                            <DashboardBoard tasks={projectFilteredTasks} onSelectTask={handleSelectTask} onUpdateTaskField={updateTaskField} />
                        ) : (
                            <DashboardTable tasks={projectFilteredTasks} onSelectTask={handleSelectTask} />
                        )}
                    </div>
                    {/* --- RENDER: TASK DETAIL VIEW DRAWER --- */}
                    <div className={`absolute inset-0 flex flex-col lg:flex-row min-h-0 bg-slate-50 transition-transform duration-300 z-10 ${currentTask ? 'translate-x-0' : 'translate-x-full'}`}>
                        {currentTask && (
                            <TaskDetail
                                task={currentTask}
                                // Core data mutations remain handled via state modifiers
                                onUpdateField={(id, field, val) => setTasks(prev => prev.map(t => t.id === id ? { ...t, [field]: val } : t))}
                                onToggleSubtask={() => { }}
                                onAddComment={() => { }}
                            />
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
};