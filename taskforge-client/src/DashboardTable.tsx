import React from 'react';
import type { Task } from './types';

interface DashboardTableProps {
    tasks: Task[];
    onSelectTask: (id: string) => void;
}

export const DashboardTable: React.FC<DashboardTableProps> = ({ tasks, onSelectTask }) => {
    return (
        <div className="bg-white border border-slate-200 rounded-2xl shadow-sm overflow-hidden w-full">
            <table className="w-full text-left border-collapse">
                <thead>
                    <tr className="bg-slate-50 border-b border-slate-200 text-[11px] font-bold uppercase text-slate-400 tracking-wider">
                        <th className="px-6 py-4">Task Name</th>
                        <th className="px-6 py-4">Priority</th>
                        <th className="px-6 py-4">Status</th>
                        <th className="px-6 py-4">Project</th>
                    </tr>
                </thead>
                <tbody className="text-xs text-slate-600 divide-y divide-slate-100">
                    {tasks.map((task) => (
                        <tr
                            key={task.id}
                            onClick={() => onSelectTask(task.id)}
                            className="border-b border-slate-100 hover:bg-slate-50 transition cursor-pointer"
                        >
                            <td className="px-6 py-4 font-semibold text-slate-900">{task.title}</td>
                            <td className="px-6 py-4">
                                <span className={`px-1.5 py-0.5 rounded text-[10px] font-semibold ${task.priority === 'High' ? 'bg-rose-50 text-rose-600' :
                                        task.priority === 'Medium' ? 'bg-amber-50 text-amber-600' : 'bg-slate-100 text-slate-600'
                                    }`}>
                                    {task.priority}
                                </span>
                            </td>
                            <td className="px-6 py-4">
                                <span className={`text-[10px] font-bold px-2 py-0.5 rounded-md ${task.status === 'In Progress' ? 'bg-emerald-100 text-emerald-700' :
                                        task.status === 'In Review' ? 'bg-amber-100 text-amber-700' : 'bg-slate-100 text-slate-700'
                                    }`}>
                                    {task.status}
                                </span>
                            </td>
                            <td className="px-6 py-4 text-slate-500 font-medium">{task.project}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
};