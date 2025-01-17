const Environment = {
    apiUrl: "http://localhost:5039/api",
    apiVersion: "v1"
};

Environment.apiUrl += `/${Environment.apiVersion}`;

export default Environment;