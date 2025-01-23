const Environment = {
    apiUrl: "https://localhost:7207/api",
    apiVersion: "v1"
};

Environment.apiUrl += `/${Environment.apiVersion}`;

export default Environment;