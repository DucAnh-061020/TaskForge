import { useState } from 'react';
import { useNavigate } from 'react-router-dom';

export const AuthPortal: React.FC = () => {
    const navigate = useNavigate();
    const API_BASE_URL = 'http://localhost:5000/';
    // State to manage whether the sign-up panel is active
    const [isSignUp, setIsSignUp] = useState(false);

    // Form input state management
    const [signInData, setSignInData] = useState({ email: '', password: '', rememberMe: false });
    const [signUpData, setSignUpData] = useState({ email: '', password: '', firstname: '', lastname: '', terms: false });

    // Handle Input Changes
    function handleSignInChange(e: React.ChangeEvent<HTMLInputElement>) {
        const { name, value, type, checked } = e.target;
        setSignInData((prev) => ({
            ...prev,
            [name]: type === 'checkbox' ? checked : value
        }));
    }

    function handleSignUpChange(e: React.ChangeEvent<HTMLInputElement>) {
        const { name, value, type, checked } = e.target;
        setSignUpData((prev) => ({ ...prev, [name]: type === 'checkbox' ? checked : value }));
    }

    // Handle Form Submissions
    async function handleSignInSubmit(e: React.FormEvent<HTMLFormElement>) {
        e.preventDefault();
        console.log('Signing In with:', signInData);

        try {
            const response = await fetch(`${API_BASE_URL}api/Auth/login`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json', // Tells backend you are sending JSON data
                },
                body: JSON.stringify({
                    email: signInData.email,
                    password: signInData.password
                }),
            });

            const data = await response.json();

            if (response.ok) {
                console.log('Sign in successful!', data);

                // Optional: Save user token (JWT) if your backend returns one
                // localStorage.setItem('token', data.token);

                // Redirect user to workspace
                navigate('/workspace');
            } else {
                // Handle backend errors (e.g., "Wrong password", "User not found")
                alert(data.message || 'Failed to sign in. Please try again.');
            }
        } catch (error) {
            console.error('Network error during sign in:', error);
            alert('Could not connect to the server. Is your backend running?');
        }
    }

    async function handleSignUpSubmit(e: React.FormEvent<HTMLFormElement>) {
        e.preventDefault();
        console.log('Signing Up with:', signUpData);
        try {
            const response = await fetch(`${API_BASE_URL}api/Auth/register`, {
                method: 'POST',
                headers: {
                    'Content-type': 'application/json',
                },
                body: JSON.stringify({
                    email: signUpData.email,
                    password: signUpData.password,
                    firstName: signUpData.firstname,
                    lastName: signUpData.lastname
                }),
            });
            const data = await response.json();
            if (response.ok) {
                alert('Registration successful! Please sign in.');
                setIsSignUp(false);
            } else {
                alert(data.message || 'Registration failed.');
            }
        } catch (error) {
            console.error('Network error during sign up:', error);
            alert('Could not connect to the server.');
        }
    }

    return (
        <div className="relative w-full h-screen bg-white overflow-hidden flex select-none antialiased">

            {/* --- SIGN IN BOX --- */}
            <div
                className={`absolute top-0 left-0 h-full w-full md:w-1/2 flex flex-col justify-center items-center px-8 sm:px-16 lg:px-24 bg-white transition-all duration-700 ease-in-out z-10 
          ${isSignUp ? 'opacity-0 pointer-events-none md:opacity-100 md:pointer-events-auto' : 'opacity-100 pointer-events-auto'}`}
            >
                <form className="w-full max-w-md space-y-6" onSubmit={handleSignInSubmit}>
                    <div className="space-y-2">
                        <h2 className="text-4xl font-extrabold text-slate-900 tracking-tight">Sign In</h2>
                        <p className="text-sm text-slate-500">Please enter your user credentials to access your portal.</p>
                    </div>

                    <div className="space-y-4">
                        <div>
                            <label htmlFor="signin-user" className="block text-sm font-semibold text-slate-700 mb-2">User</label>
                            <input
                                type="email"
                                id="signin-user"
                                name="email"
                                required
                                autoComplete="email"
                                placeholder="Email address"
                                value={signInData.email}
                                onChange={handleSignInChange}
                                className="w-full px-4 py-3 rounded-xl border border-slate-300 placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 transition duration-200 bg-slate-50/50" />
                        </div>

                        <div>
                            <div className="flex items-center justify-between mb-2">
                                <label htmlFor="signin-password" className="block text-sm font-semibold text-slate-700">Password</label>
                                {/*<a href="#forgot" className="text-sm font-medium text-emerald-600 hover:text-emerald-500">Forgot?</a>*/}
                            </div>
                            <input
                                type="password"
                                id="signin-password"
                                name="password"
                                required
                                autoComplete="current-password"
                                placeholder="******"
                                value={signInData.password}
                                onChange={handleSignInChange}
                                className="w-full px-4 py-3 rounded-xl border border-slate-300 placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 transition duration-200 bg-slate-50/50" />
                        </div>
                    </div>

                    <div className="flex items-center">
                        <input
                            id="remember-me"
                            name="rememberMe"
                            type="checkbox"
                            checked={signInData.rememberMe}
                            onChange={handleSignInChange}
                            className="h-4 w-4 rounded border-slate-300 text-emerald-600 focus:ring-emerald-500 cursor-pointer" />
                        <label htmlFor="remember-me" className="ml-2 block text-sm text-slate-600 cursor-pointer select-none">
                            Remember this device
                        </label>
                    </div>

                    <button
                        type="submit"
                        className="w-full bg-emerald-600 hover:bg-emerald-700 text-white font-bold py-3.5 px-4 rounded-xl shadow-lg shadow-emerald-100 transition duration-200 active:scale-[0.99]"
                    >
                        Sign In
                    </button>

                    <p className="text-center text-sm text-slate-500 mt-6 md:hidden">
                        New around here?{' '}
                        <button type="button" onClick={() => setIsSignUp(true)} className="font-bold text-emerald-600 hover:underline">
                            Create an account
                        </button>
                    </p>
                </form>
            </div>

            {/* --- SIGN UP BOX --- */}
            <div
                className={`absolute top-0 right-0 h-full w-full md:w-1/2 flex flex-col justify-center items-center px-8 sm:px-16 lg:px-24 bg-white transition-all duration-700 ease-in-out z-10 
          ${isSignUp ? 'opacity-100 pointer-events-auto' : 'opacity-0 pointer-events-none md:opacity-100 md:pointer-events-auto'}`}
            >
                <form className="w-full max-w-md space-y-6" onSubmit={handleSignUpSubmit}>
                    <div className="space-y-2">
                        <h2 className="text-4xl font-extrabold text-slate-900 tracking-tight">Sign Up</h2>
                        <p className="text-sm text-slate-500">Set up your new user profile to get started.</p>
                    </div>

                    <div className="space-y-4">
                        <div>
                            <label htmlFor="signup-user" className="block text-sm font-semibold text-slate-700 mb-2">User</label>
                            <input
                                type="email"
                                id="signup-user"
                                name="email"
                                required
                                autoComplete="email"
                                placeholder="Email Address"
                                value={signUpData.email}
                                onChange={handleSignUpChange}
                                className="w-full px-4 py-3 rounded-xl border border-slate-300 placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 transition duration-200 bg-slate-50/50" />
                        </div>

                        <div>
                            <label htmlFor="signup-password" className="block text-sm font-semibold text-slate-700 mb-2">Password</label>
                            <input
                                type="password"
                                id="signup-password"
                                name="password"
                                required
                                autoComplete="new-password"
                                placeholder="Create a strong password"
                                value={signUpData.password}
                                onChange={handleSignUpChange}
                                className="w-full px-4 py-3 rounded-xl border border-slate-300 placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 transition duration-200 bg-slate-50/50" />
                        </div>
                        <div>
                            <label htmlFor="signup-firstname" className="block text-sm font-semibold text-slate-700 mb-2">Firstname</label>
                            <input
                                type="text"
                                id="signup-firstname"
                                name="firstname"
                                required
                                autoComplete="firstname"
                                placeholder="Firstname"
                                value={signUpData.firstname}
                                onChange={handleSignUpChange}
                                className="w-full px-4 py-3 rounded-xl border border-slate-300 placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 transition duration-200 bg-slate-50/50" />
                        </div>
                        <div>
                            <label htmlFor="signup-lastname" className="block text-sm font-semibold text-slate-700 mb-2">Lastname</label>
                            <input
                                type="text"
                                id="signup-lastname"
                                name="lastname"
                                required
                                autoComplete="lastname"
                                placeholder="Lastname"
                                value={signUpData.lastname}
                                onChange={handleSignUpChange}
                                className="w-full px-4 py-3 rounded-xl border border-slate-300 placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 transition duration-200 bg-slate-50/50" />
                        </div>
                    </div>

                    <div className="flex items-start">
                        <input
                            id="terms"
                            name="terms"
                            type="checkbox"
                            required
                            checked={signUpData.terms}
                            onChange={handleSignUpChange}
                            className="mt-0.5 h-4 w-4 rounded border-slate-300 text-emerald-600 focus:ring-emerald-500 cursor-pointer" />
                        <label htmlFor="terms" className="ml-2 block text-sm text-slate-600 cursor-pointer select-none">
                            I agree to the platform Terms of Service
                        </label>
                    </div>

                    <button
                        type="submit"
                        className="w-full bg-emerald-600 hover:bg-emerald-700 text-white font-bold py-3.5 px-4 rounded-xl shadow-lg shadow-emerald-100 transition duration-200 active:scale-[0.99]"
                    >
                        Sign Up
                    </button>

                    <p className="text-center text-sm text-slate-500 mt-6 md:hidden">
                        Already registered?{' '}
                        <button type="button" onClick={() => setIsSignUp(false)} className="font-bold text-emerald-600 hover:underline">
                            Sign back in
                        </button>
                    </p>
                </form>
            </div>

            {/* --- DESKTOP SLIDING OVERLAY PANEL --- */}
            <div
                className={`hidden md:block absolute top-0 left-1/2 w-1/2 h-full overflow-hidden transition-transform duration-700 ease-[cubic-bezier(0.4,0,0.2,1)] z-30 
          ${isSignUp ? '-translate-x-full' : 'translate-x-0'}`}
            >
                <div
                    className={`bg-gradient-to-tr from-slate-900 via-emerald-950 to-emerald-900 text-white relative -left-full h-full w-[200%] transform transition-transform duration-700 ease-[cubic-bezier(0.4,0,0.2,1)] 
            ${isSignUp ? 'translate-x-1/2' : 'translate-x-0'}`}
                >
                    {/* Left Panel Content */}
                    <div
                        className={`absolute top-0 flex flex-col items-center justify-center h-full w-1/2 px-16 lg:px-24 text-center space-y-6 transition-transform duration-700 ease-[cubic-bezier(0.4,0,0.2,1)] 
              ${isSignUp ? 'translate-x-0' : '-translate-x-[20%]'}`}
                    >
                        <h2 className="text-4xl font-extrabold tracking-tight">Already Have an Account?</h2>
                        <p className="text-emerald-200/80 leading-relaxed max-w-sm text-sm">
                            To jump right back into your custom workspace, please sign in using your existing user profile configuration.
                        </p>
                        <button
                            type="button"
                            onClick={() => setIsSignUp(false)}
                            className="bg-transparent border-2 border-white/80 hover:bg-white hover:text-emerald-950 text-white font-bold px-12 py-3 rounded-xl transition duration-200 active:scale-95 mt-4"
                        >
                            Sign In
                        </button>
                    </div>

                    {/* Right Panel Content */}
                    <div
                        className={`absolute top-0 right-0 flex flex-col items-center justify-center h-full w-1/2 px-16 lg:px-24 text-center space-y-6 transition-transform duration-700 ease-[cubic-bezier(0.4,0,0.2,1)] 
              ${isSignUp ? 'translate-x-[20%]' : 'translate-x-0'}`}
                    >
                        <h2 className="text-4xl font-extrabold tracking-tight">First Time Here?</h2>
                        <p className="text-emerald-200/80 leading-relaxed max-w-sm text-sm">
                            Create your personal username and configure your security settings to establish your profile instantly.
                        </p>
                        <button
                            type="button"
                            onClick={() => setIsSignUp(true)}
                            className="bg-transparent border-2 border-white/80 hover:bg-white hover:text-emerald-950 text-white font-bold px-12 py-3 rounded-xl transition duration-200 active:scale-95 mt-4"
                        >
                            Sign Up
                        </button>
                    </div>
                </div>
            </div>

        </div>
    );
}