const BASE_URL = "http://localhost:5118/api";

export async function apiFetch<T>(
  path: string,
  options: RequestInit = {},
): Promise<T> {
  const response = await fetch(`${BASE_URL}${path}`, {
    headers: { "Content-Type": "application/json" },
    ...options,
  });

  const responseData = await response.json();

  if (!response.ok) {
    const { errorCode, errorMessage } = responseData.error;

    throw new Error(`${errorCode}: ${errorMessage}`);
  }

  return responseData as T;
}
