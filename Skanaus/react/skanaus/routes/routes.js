import React, { Component, ErrorInfo } from 'react';
import type { Router as RemixRouter } from '@remix-run/router';
import { Outlet, RouteObject } from 'react-router';
import { createBrowserRouter } from 'react-router-dom';
import NavigationBar from '../components/NavigationBar/NavigationBar';
import Sidebar from '../components/Sidebar/Sidebar';
import { Endpoint } from './Endpoint';
import MainPage from '../features/MainPage/MainPage';
import NotFound from '../components/ErrorPage/NotFound';
import Login from '../components/Login/Login';
import Register from '../components/Register/Register';
import ManageTeam from '../features/ManageTeam/ManageTeam';
import AddSprint from '../features/AddSprint/AddSprint';
import ProtectedRoute from './ProtectedRoute';
import TopicPosts from '../components/TopicPosts/TopicPosts';
import PostComments from '../components/PostComments/PostComments';

type MyErrorBoundaryProps = {
    children: React.ReactNode;
};

class MyErrorBoundary extends Component<MyErrorBoundaryProps> {
    state = {
        hasError: false,
    };

    static getDerivedStateFromError() {
        return { hasError: true };
    }

    componentDidCatch(error: Error, errorInfo: ErrorInfo) {
        console.error(error, errorInfo);
    }

    render() {
        if (this.state.hasError) {
            return <NotFound />;
        }

        return this.props.children;
    }
}

const routeOptions: RouteObject[] = [
    {
        path: '/*',
        element: (
            <>
                <NavigationBar />
                <Sidebar>
                    <MyErrorBoundary>
                        <Outlet />
                    </MyErrorBoundary>
                </Sidebar>
            </>
        ),
        children: [
            {
                path: Endpoint.MAIN_PAGE,
                element: (
                    <ProtectedRoute path={Endpoint.MAIN_PAGE} element={<MainPage />} />
                ),
            },
            {
                path: Endpoint.ADD_SPRINT,
                element: (
                    <ProtectedRoute path={Endpoint.ADD_SPRINT} element={<AddSprint />} />
                ),
            },
            {
                path: Endpoint.MANAGE_TEAM,
                element: (
                    <ProtectedRoute
                        path={Endpoint.MANAGE_TEAM}
                        element={<ManageTeam />}
                    />
                ),
            },
            {
                path: `${Endpoint.TOPIC}/:topicId`,
                element: (
                    <ProtectedRoute
                        path={`${Endpoint.TOPIC}/:topicId`}
                        element={<TopicPosts />}
                    />
                ),
            },
            {
                path: `${Endpoint.POST}/:topicId/post/:postId`,
                element: (
                    <ProtectedRoute
                        path={`${Endpoint.POST}/:postId`}
                        element={<PostComments />}
                    />
                ),
            },
            {
                path: `${Endpoint.TOPIC}/:topicId/post/:postId`,
                element: (
                    <ProtectedRoute
                        path={`${Endpoint.TOPIC}/:topicId/post/:postId`}
                        element={<TopicPosts />}
                    />
                ),
            },
        ],
    },
    {
        path: Endpoint.LOGIN,
        element: <Login />,
    },
    {
        path: Endpoint.REGISTER,
        element: <Register />,
    },
    {
        path: '*',
        element: <NotFound />,
    },
];

export const routes: RemixRouter = createBrowserRouter(routeOptions);