import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

// Configuração mínima do bundler; proxy e aliases podem ser adicionados depois.
export default defineConfig({ plugins: [react()], server: { port: 5173, host: 'localhost' } });
