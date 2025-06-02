import React from 'react';

/**
 * ChunkErrorBoundary
 *
 * React error boundary for catching and handling dynamic chunk loading errors (e.g., ChunkLoadError).
 * Displays a fallback UI with a reload option if a chunk fails to load.
 *
 * Usage:
 * <ChunkErrorBoundary>
 *   <YourComponent />
 * </ChunkErrorBoundary>
 */
class ChunkErrorBoundary extends React.Component {
  constructor(props) {
    super(props);
    this.state = { hasError: false };
  }

  static getDerivedStateFromError(error) {
    if (
      error.name === 'ChunkLoadError' ||
      error.message.includes('Failed to fetch dynamically imported module')
    ) {
      return { hasError: true };
    }
    return { hasError: false };
  }

  render() {
    if (this.state.hasError) {
      return (
        <div className="flex flex-col items-center justify-center py-10">
          <p className="mb-4 text-red-600 font-semibold">
            Something went wrong while loading the page.
          </p>
          <button
            className="bg-black text-white px-4 py-2 rounded"
            onClick={() => window.location.reload()}
          >
            Reload
          </button>
        </div>
      );
    }

    return this.props.children;
  }
}

export default ChunkErrorBoundary;
