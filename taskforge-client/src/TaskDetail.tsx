import React, { useState } from 'react';
import type { Task } from './types';

interface TaskDetailProps {
    task: Task;
    onUpdateField: <K extends keyof Task>(id: string, field: K, value: Task[K]) => void;
    onToggleSubtask: (taskId: string, subtaskId: string) => void;
    onAddComment: (taskId: string, commentText: string) => void;
}

export const TaskDetail: React.FC<TaskDetailProps> = ({ task, onUpdateField, onToggleSubtask, onAddComment }) => {
    const [commentText, setCommentText] = useState<string>('');

    const submitComment = () => {
        if (!commentText.trim()) return;
        onAddComment(task.id, commentText.trim());
        setCommentText('');
    };

    const handleKeyDown = (e: React.KeyboardEvent<HTMLTextAreaElement>) => {
        if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault();
            submitComment();
        }
    };

    const completedSubtasks = task.subtasks.filter((st) => st.done).length;

    return (
        <>
            {/* Left Side Container: Documentation Feeds */}
            <div className="flex-1 p-6 md:p-8 space-y-8 overflow-y-auto border-b lg:border-b-0 lg:border-r border-slate-200/60">
                <div className="space-y-2">
                    <div className="flex items-center gap-2 text-xs font-bold text-emerald-600 bg-emerald-50 border border-emerald-100 px-2 py-0.5 rounded w-max">
                        {task.code}
                    </div>
                    <input
                        type="text"
                        value={task.title}
                        onChange={(e) => onUpdateField(task.id, 'title', e.target.value)}
                        className="w-full text-2xl md:text-3xl font-extrabold text-slate-900 tracking-tight bg-transparent border-none p-0 focus:outline-none focus:ring-2 focus:ring-emerald-500/20 rounded"
                    />
                    <p className="text-xs text-slate-400">
                        Workspace Node Managed by <span className="font-semibold text-slate-600">{task.assignee}</span>
                    </p>
                </div>

                <div className="space-y-3">
                    <h3 className="text-[10px] font-bold uppercase text-slate-400 tracking-wider">Description Documentation</h3>
                    <div className="bg-white border border-slate-200 rounded-2xl p-4 text-sm text-slate-600 shadow-sm transition focus-within:ring-2 focus-within:ring-emerald-500/10 focus-within:border-emerald-500">
                        <textarea
                            value={task.desc}
                            onChange={(e) => onUpdateField(task.id, 'desc', e.target.value)}
                            rows={4}
                            className="w-full text-xs text-slate-600 leading-relaxed bg-transparent border-none p-0 focus:outline-none resize-none"
                        />
                    </div>
                </div>

                <div className="space-y-4">
                    <div className="flex items-center justify-between">
                        <h3 className="text-[10px] font-bold uppercase text-slate-400 tracking-wider">Subtasks Progress Tracking</h3>
                        <span className="text-xs font-semibold text-emerald-600 bg-emerald-50 px-2 py-0.5 rounded">
                            {completedSubtasks}/{task.subtasks.length} Done
                        </span>
                    </div>

                    <div className="space-y-2 bg-white border border-slate-200 rounded-2xl p-3 shadow-sm">
                        {task.subtasks.length === 0 ? (
                            <p className="text-xs text-slate-400 italic p-2.5">No subtasks found for this issue block.</p>
                        ) : (
                            task.subtasks.map((st) => (
                                <label key={st.id} className="flex items-center justify-between p-2.5 rounded-xl hover:bg-slate-50 cursor-pointer transition select-none">
                                    <span className={`flex items-center gap-3 text-xs font-medium ${st.done ? 'text-slate-400 line-through' : 'text-slate-700'}`}>
                                        <input
                                            type="checkbox"
                                            checked={st.done}
                                            onChange={() => onToggleSubtask(task.id, st.id)}
                                            className="h-4 w-4 rounded text-emerald-600 focus:ring-emerald-500/30 border-slate-300 cursor-pointer"
                                        />
                                        {st.text}
                                    </span>
                                </label>
                            ))
                        )}
                    </div>
                </div>

                <div className="space-y-4 pt-4 border-t border-slate-200/80">
                    <h3 className="text-[10px] font-bold uppercase text-slate-400 tracking-wider">Discussion Feed Streams</h3>
                    <div className="space-y-4 max-h-[200px] overflow-y-auto pr-2">
                        {task.comments.map((comment) => (
                            <div key={comment.id} className="flex gap-3 text-xs leading-relaxed">
                                <div className="h-7 w-7 bg-amber-500 rounded-full flex items-center justify-center text-white font-bold shrink-0">
                                    {comment.avatar}
                                </div>
                                <div className="space-y-1 bg-white border border-slate-200 p-3 rounded-2xl flex-1 shadow-sm">
                                    <div className="flex items-center justify-between">
                                        <span className="font-bold text-slate-900">{comment.author}</span>
                                        <span className="text-[10px] text-slate-400">{comment.timestamp}</span>
                                    </div>
                                    <p className="text-slate-600 font-medium">{comment.text}</p>
                                </div>
                            </div>
                        ))}
                    </div>

                    <div className="flex gap-3 items-start pt-2">
                        <div className="h-7 w-7 bg-emerald-600 rounded-full flex items-center justify-center text-white font-bold text-xs shrink-0">ME</div>
                        <div className="flex-1 relative">
                            <textarea
                                rows={1}
                                placeholder="Add an update note..."
                                value={commentText}
                                onChange={(e) => setCommentText(e.target.value)}
                                onKeyDown={handleKeyDown}
                                className="w-full text-xs bg-white border border-slate-200 rounded-xl pl-4 pr-12 py-3.5 focus:outline-none focus:ring-2 focus:ring-emerald-500/20 shadow-sm transition resize-none"
                            />
                            <button
                                onClick={submitComment}
                                className="absolute right-2.5 top-2.5 p-1.5 bg-emerald-600 hover:bg-emerald-700 text-white rounded-lg transition shadow-sm"
                            >
                                <svg className="h-3.5 w-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2.5">
                                    <path strokeLinecap="round" strokeLinejoin="round" d="M14 5l7 7m0 0l-7 7m7-7H3" />
                                </svg>
                            </button>
                        </div>
                    </div>
                </div>
            </div>

            {/* Right Side Container: Metadata Control Columns */}
            <div className="w-full lg:w-64 bg-white p-6 md:p-8 space-y-5 shrink-0 border-t lg:border-t-0 lg:border-l border-slate-200/60 text-xs overflow-y-auto">
                <div className="space-y-1.5">
                    <label className="text-[10px] font-bold uppercase text-slate-400 tracking-wider block">Lifecycle Status</label>
                    <select
                        value={task.status}
                        onChange={(e) => onUpdateField(task.id, 'status', e.target.value as Task['status'])}
                        className="w-full bg-slate-50 border border-slate-200 rounded-xl px-2.5 py-2 font-bold text-emerald-600 shadow-sm focus:outline-none"
                    >
                        <option value="To Do">📋 To Do</option>
                        <option value="In Progress">⚡ In Progress</option>
                        <option value="In Review">🔍 In Review</option>
                        <option value="Completed">✅ Completed</option>
                    </select>
                </div>

                <div className="space-y-1.5">
                    <label className="text-[10px] font-bold uppercase text-slate-400 tracking-wider block">Priority Rank</label>
                    <select
                        value={task.priority}
                        onChange={(e) => onUpdateField(task.id, 'priority', e.target.value as Task['priority'])}
                        className="w-full bg-slate-50 border border-slate-200 rounded-xl px-2.5 py-2 font-semibold text-slate-700 shadow-sm focus:outline-none"
                    >
                        <option value="Low">Low</option>
                        <option value="Medium">Medium</option>
                        <option value="High">High</option>
                    </select>
                </div>

                <div className="pt-3 border-t border-slate-200/60 space-y-2">
                    <div className="flex justify-between"><span className="text-slate-400 font-medium">Assignee:</span><span className="font-semibold text-slate-700">{task.assignee}</span></div>
                    <div className="flex justify-between"><span className="text-slate-400 font-medium">Project Node:</span><span className="font-semibold text-slate-700">{task.project}</span></div>
                </div>

                <div className="space-y-3 pt-3 border-t border-slate-200/60">
                    <div className="flex justify-between items-center">
                        <span className="text-[10px] font-bold uppercase text-slate-400 tracking-wider">Start Date</span>
                        <input
                            type="date"
                            value={task.startDate}
                            onChange={(e) => onUpdateField(task.id, 'startDate', e.target.value)}
                            className="bg-transparent border-0 font-medium text-slate-700 focus:outline-none text-right cursor-pointer"
                        />
                    </div>
                    <div className="flex justify-between items-center">
                        <span className="text-[10px] font-bold uppercase text-slate-400 tracking-wider">Due Date</span>
                        <input
                            type="date"
                            value={task.dueDate}
                            onChange={(e) => onUpdateField(task.id, 'dueDate', e.target.value)}
                            className="bg-transparent border-0 font-bold text-rose-600 focus:outline-none text-right cursor-pointer"
                        />
                    </div>
                </div>

                <div className="space-y-2 pt-3 border-t border-slate-200/60">
                    <label className="text-[10px] font-bold uppercase text-slate-400 tracking-wider block">Logged Time Index</label>
                    <div className="bg-slate-50 border border-slate-200 rounded-xl p-3 shadow-sm space-y-2">
                        <div className="flex justify-between text-[11px] font-semibold text-slate-600">
                            <span>Logged: <b className="text-slate-900">{task.loggedTime}h</b></span>
                            <span>Estimate: <b className="text-slate-400">{task.estimatedTime}h</b></span>
                        </div>
                        <div className="w-full bg-slate-200 h-1.5 rounded-full overflow-hidden">
                            <div
                                className="bg-emerald-600 h-full rounded-full"
                                style={{ width: `${(task.loggedTime / task.estimatedTime) * 100}%` }}
                            />
                        </div>
                    </div>
                </div>
            </div>
        </>
    );
};