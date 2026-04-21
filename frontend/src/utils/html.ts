/**
 * Удаляет HTML теги и декодирует HTML entities
 */
export function stripHtmlTags(html: string): string {
  if (!html) return '';

  // Декодируем HTML entities
  const textArea = document.createElement('textarea');
  textArea.innerHTML = html;
  let text = textArea.value;

  // Удаляем все HTML теги
  text = text.replace(/<[^>]*>/g, ' ');

  // Убираем лишние пробелы
  text = text.replace(/\s+/g, ' ').trim();

  return text;
}

/**
 * Обрезает текст до определённой длины с многоточием
 */
export function truncateText(text: string, maxLength: number): string {
  if (text.length <= maxLength) return text;
  return text.substring(0, maxLength) + '...';
}
