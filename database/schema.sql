-- Core tables for infrastructure components
CREATE TABLE IF NOT EXISTS nodes (
    node_id CHAR(36) PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    type VARCHAR(50) NOT NULL, -- server, API, service, database, etc.
    status VARCHAR(50) DEFAULT 'unknown', -- healthy, warning, critical, unknown
    metadata JSON, -- flexible storage for component-specific attributes (MySQL 5.7+)
    position_x FLOAT,
    position_y FLOAT,
    position_z FLOAT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS connections (
    connection_id CHAR(36) PRIMARY KEY,
    source_node_id CHAR(36),
    target_node_id CHAR(36),
    type VARCHAR(50),
    status VARCHAR(50) DEFAULT 'unknown',
    metadata JSON,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (source_node_id) REFERENCES nodes(node_id) ON DELETE CASCADE,
    FOREIGN KEY (target_node_id) REFERENCES nodes(node_id) ON DELETE CASCADE
);

-- Monitoring and metrics
CREATE TABLE IF NOT EXISTS metrics (
    metric_id CHAR(36) PRIMARY KEY,
    node_id CHAR(36),
    metric_name VARCHAR(255) NOT NULL,
    metric_value FLOAT,
    timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (node_id) REFERENCES nodes(node_id) ON DELETE CASCADE
);

-- Alerts and incidents
CREATE TABLE IF NOT EXISTS alerts (
    alert_id CHAR(36) PRIMARY KEY,
    node_id CHAR(36),
    connection_id CHAR(36),
    severity VARCHAR(50) NOT NULL,
    message TEXT NOT NULL,
    status VARCHAR(50) DEFAULT 'active',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    resolved_at TIMESTAMP,
    FOREIGN KEY (node_id) REFERENCES nodes(node_id) ON DELETE SET NULL,
    FOREIGN KEY (connection_id) REFERENCES connections(connection_id) ON DELETE SET NULL
);

-- User interactions and sessions
CREATE TABLE IF NOT EXISTS users (
    user_id CHAR(36) PRIMARY KEY,
    username VARCHAR(255) UNIQUE NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    role VARCHAR(50) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS vr_sessions (
    session_id CHAR(36) PRIMARY KEY,
    user_id CHAR(36),
    start_time TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    end_time TIMESTAMP,
    session_data JSON,
    FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE
);

-- Collaboration features
CREATE TABLE IF NOT EXISTS annotations (
    annotation_id CHAR(36) PRIMARY KEY,
    user_id CHAR(36),
    node_id CHAR(36),
    content TEXT NOT NULL,
    position_x FLOAT,
    position_y FLOAT,
    position_z FLOAT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE,
    FOREIGN KEY (node_id) REFERENCES nodes(node_id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS system_logs (
    log_id CHAR(36) PRIMARY KEY, -- UUID stored as string
    node_id CHAR(36), -- must match the referenced type
    log_level VARCHAR(20) NOT NULL, -- INFO, WARNING, ERROR, DEBUG
    message TEXT NOT NULL,
    timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    metadata JSON, -- JSON instead of JSONB
    FOREIGN KEY (node_id) REFERENCES nodes(node_id)
);
