const Environment = {
	rootApiUrl: "https://localhost:7207",
	apiUrl: "api",
	apiUrlVersioned: "",
	apiVersion: "v1",
};

Environment.apiUrl = `${Environment.rootApiUrl}/${Environment.apiUrl}`;
Environment.apiUrlVersioned = `${Environment.apiUrl}/${Environment.apiVersion}`;

console.log("Versioned api:",Environment.apiUrlVersioned);

export default Environment;