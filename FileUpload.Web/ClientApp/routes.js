import * as React from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { FileUpload } from './components/FileUpload';

export const routes = <Layout>
    <Route exact path='/' component={ Home } />
    <Route path='/upload' component={ FileUpload } />
</Layout>;
