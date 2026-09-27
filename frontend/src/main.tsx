import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { App } from './app/app';

// Bootstrap do React, mantido pequeno para facilitar testes e evolução.
createRoot(document.getElementById('root')!).render(<StrictMode><App /></StrictMode>);
