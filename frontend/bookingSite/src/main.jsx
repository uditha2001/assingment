import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.jsx'
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import ChunkErrorBoundary from './errorBoundary/ChunkErrorBundary.jsx';
createRoot(document.getElementById('root')).render(
  <StrictMode>
    <ChunkErrorBoundary>
    <BrowserRouter>
    <App />
    </BrowserRouter>
    </ChunkErrorBoundary>
  </StrictMode>,
)
