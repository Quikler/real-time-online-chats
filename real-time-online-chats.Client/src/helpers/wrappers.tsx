import { Observable, Subscriber } from "rxjs";

export function observableWithAbort<T>(
  func: (abortController: AbortController, observer: Subscriber<T>) => void
): {
  observable: Observable<any>;
  abort: (reason?: any) => void;
} {
  const abortController = new AbortController();

  const observable = new Observable((observer) => {
    func(abortController, observer);

    // Return cleanup function to handle unsubscription
    return () => abortController.abort();
  });

  return {
    observable,
    abort: (reason: any) => abortController.abort(reason),
  };
}
