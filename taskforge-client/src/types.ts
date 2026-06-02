export interface SubTask {
    id: string;
    text: string;
    done: boolean;
}

export interface Comment {
    id: string;
    author: string;
    avatar: string;
    text: string;
    timestamp: string;
}

export interface Task {
    id: string;
    code: string;
    title: string;
    desc: string;
    priority: 'Low' | 'Medium' | 'High';
    status: 'To Do' | 'In Progress' | 'In Review' | 'Completed';
    isMine: boolean;
    project: string;
    projectType: string;
    assignee: string;
    startDate: string;
    dueDate: string;
    loggedTime: number;
    estimatedTime: number;
    subtasks: SubTask[];
    comments: Comment[];
}

export type FilterType = 'all' | 'mine';