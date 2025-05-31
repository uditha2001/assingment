import React from 'react';

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
        <div>
          <p>Something went wrong while loading the page.</p>
          <button onClick={() => window.location.reload()}>Reload</button>
        </div>
      );
    }

    return this.props.children;
  }
}

export default ChunkErrorBoundary;
