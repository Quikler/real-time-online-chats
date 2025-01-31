import { Link } from "react-router-dom";

const Forbidden = () => {
  return (
    <div className="dark:bg-slate-900 bg-gradient-to-br from-slate-50 to-slate-100 min-h-screen flex items-center justify-center">
      <div className="w-full">
        <div className="py-8 px-4 lg:py-16 lg:px-6">
          <div className="mx-auto max-w-screen-sm text-center relative">
            <div className="absolute -top-16 left-1/2 -translate-x-1/2 opacity-20 dark:opacity-10">
              <svg viewBox="0 0 200 200" className="w-64 h-64" xmlns="http://www.w3.org/2000/svg">
                <path
                  fill="currentColor"
                  d="M45.8,-76.4C58.9,-70.4,68.7,-58.1,75.8,-44.3C82.9,-30.5,87.4,-15.3,87.9,0.2C88.4,15.6,84.9,31.3,77.1,44.6C69.3,57.9,57.2,68.9,43.3,75.8C29.4,82.7,13.7,85.5,-0.7,86.7C-15.1,87.9,-30.2,87.5,-43.8,81.1C-57.3,74.7,-69.3,62.3,-76.1,47.9C-82.9,33.5,-84.5,17.3,-83.9,1.1C-83.3,-15.2,-80.5,-30.3,-72.8,-42.8C-65.1,-55.2,-52.5,-64.9,-38.7,-70.4C-24.9,-75.8,-9.9,-77,3.7,-82.8C17.3,-88.6,34.6,-99,45.8,-76.4Z"
                  transform="translate(100 100)"
                />
              </svg>
            </div>
            <h1 className="mb-4 text-7xl tracking-tight font-extrabold lg:text-9xl bg-gradient-to-r from-slate-600 to-slate-800 dark:from-slate-300 dark:to-slate-100 bg-clip-text text-transparent">
              403
            </h1>
            <p className="mb-4 text-3xl tracking-tight font-bold text-slate-800 md:text-4xl dark:text-slate-200">
              Access Forbidden
            </p>
            <p className="mb-4 text-lg font-light text-slate-600 dark:text-slate-400">
              You don't have permission to view this page
            </p>
            <Link
              to="/"
              className="inline-flex items-center justify-center gap-2 text-slate-100 bg-slate-600 hover:bg-slate-700 focus:ring-2 focus:ring-offset-2 focus:ring-slate-500 font-medium rounded-lg text-sm px-5 py-2.5 transition-all duration-200 dark:bg-slate-300 dark:text-slate-900 dark:hover:bg-slate-200 dark:focus:ring-slate-600 my-4 shadow-lg hover:shadow-slate-400/30 dark:hover:shadow-slate-900/30"
            >
              Back to Homepage
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Forbidden;
