import { useState } from 'react';
import type { Task } from './types';

interface DashboardBoardProps {
    tasks: Task[];
    onSelectTask: (id: string) => void;
    onUpdateTaskField: <K extends keyof Task>(id: string, field: K, value: Task[K]) => void;
}

const COLUMNS: Array<Task['status']> = ['To Do', 'In Progress', 'In Review', 'Completed'];

export const DashboardBoard: React.FC<DashboardBoardProps> = ({ tasks, onSelectTask, onUpdateTaskField }) => {
    const [dragOverColumn, setDragOverColumn] = useState<Task['status'] | null>(null);

    const handleDragStart = (e: React.DragEvent<HTMLDivElement>, id: string) => {
        e.dataTransfer.setData('text/plain', id);
    };

    const handleDrop = (e: React.DragEvent<HTMLDivElement>, status: Task['status']) => {
        e.preventDefault();
        setDragOverColumn(null);
        const cardId = e.dataTransfer.getData('text/plain');
        if (cardId) {
            onUpdateTaskField(cardId, 'status', status);
        }
    };

    return (
        <div className="flex gap-5 items-start min-w-[900px] pb-4 h-[calc(100vh-14rem)] overflow-x-auto">
            {COLUMNS.map((colName) => {
                const columnTasks = tasks.filter((t) => t.status === colName);
                const isOver = dragOverColumn === colName;

                return (
                    <div
                        key={colName}
                        onDragOver={(e) => { e.preventDefault(); setDragOverColumn(colName); }}
                        onDragLeave={() => setDragOverColumn(null)}
                        onDrop={(e) => handleDrop(e, colName)}
                        className={`w-72 rounded-2xl flex flex-col max-h-full border shadow-sm shrink-0 transition-colors duration-200
              ${isOver ? 'bg-emerald-50/40 border-emerald-300' : 'bg-slate-100 border-slate-200/60'}`}
                    >
                        <div className="p-4 flex items-center justify-between border-b border-slate-200 bg-white/60 rounded-t-2xl">
                            <span className={`text-[10px] font-bold px-2 py-0.5 rounded-md ${colName === 'To Do' ? 'bg-slate-200 text-slate-700' :
                                    colName === 'In Progress' ? 'bg-emerald-100 text-emerald-700' :
                                        colName === 'In Review' ? 'bg-amber-100 text-amber-700' : 'bg-emerald-100 text-emerald-700'
                                }`}>
                                {colName}
                            </span>
                            <span className="text-xs font-bold text-slate-400">{columnTasks.length}</span>
                        </div>

                        <div className="p-3 space-y-3 flex-1 overflow-y-auto rounded-b-2xl min-h-[200px]">
                            {columnTasks.map((task) => (
                                <div
                                    key={task.id}
                                    draggable
                                    onDragStart={(e) => handleDragStart(e, task.id)}
                                    onClick={() => onSelectTask(task.id)}
                                    className="bg-white border border-slate-200 rounded-xl p-4 shadow-sm hover:shadow-md transition cursor-pointer flex flex-col space-y-3"
                                >
                                    <div className="space-y-1">
                                        <div className="flex justify-between items-center">
                                            <h4 className="text-sm font-bold text-slate-800 leading-tight">{task.title}</h4>
                                            {task.isMine && <span className="h-1.5 w-1.5 rounded-full bg-emerald-500" />}
                                        </div>
                                        <p className="text-xs text-slate-400 line-clamp-2 leading-relaxed">{task.desc}</p>
                                    </div>
                                    <div className="flex items-center justify-between pt-1 border-t border-slate-100 text-[10px] font-semibold">
                                        <span className={`px-1.5 py-0.5 rounded ${task.priority === 'High' ? 'bg-rose-50 text-rose-600' :
                                                task.priority === 'Medium' ? 'bg-amber-50 text-amber-600' : 'bg-slate-100 text-slate-600'
                                            }`}>
                                            {task.priority}
                                        </span>
                                        <span className="text-slate-400 italic">Status: {task.status}</span>
                                    </div>
                                </div>
                            ))}
                        </div>
                    </div>
                );
            })}
        </div>
    );
};