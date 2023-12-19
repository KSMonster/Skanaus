import React, { useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom'; // Import useNavigate from react-router-dom

const Login = () => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [response, setResponse] = useState('');
    const navigate = useNavigate(); // Create navigate object

    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            const result = await axios.post('https://octopus-app-y47u2.ondigitalocean.app/api/login', {
                userName: username,
                password: password,
            });

            // Extract access token and refresh token from the response
            const accessToken = result.data.accessToken;
            const refreshToken = result.data.refreshToken;

            // Save tokens (you can save them to state or local storage)
            // For simplicity, saving them to local storage in this example
            localStorage.setItem('accessToken', accessToken);
            localStorage.setItem('refreshToken', refreshToken);

            // Redirect to the "/kitchens" route
            navigate('/kitchens');

        } catch (error) {
            console.error('Error:', error);
            setResponse('Error: ' + error.message);
        }
    };

    return (
        <div>
            <h1>Login</h1>
            <form onSubmit={handleSubmit}>
                <label>
                    Username:
                    <input type="text" value={username} onChange={(e) => setUsername(e.target.value)} />
                </label>
                <br />
                <label>
                    Password:
                    <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} />
                </label>
                <br />
                <button type="submit">Login</button>
            </form>
            <h2>Response:</h2>
            <pre>{response}</pre>
        </div>
    );
};

export default Login;
