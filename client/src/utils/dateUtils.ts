// UTC to local time
export function formatUtcToLocal(dateString: string): string {
  if (!dateString) return '';
  
  const utcDateString = dateString.endsWith('Z') ? dateString : `${dateString}Z`;
  const date = new Date(utcDateString);
  
  if (isNaN(date.getTime())) {
    return dateString;
  }
  
  return date.toLocaleString();
}
