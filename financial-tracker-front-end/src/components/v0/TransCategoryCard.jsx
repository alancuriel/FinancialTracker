"use client";

export default function TransCategoryCard({ name, color }) {
  const categoryName = name;
  const Icon = () => (
    <svg
      className="w-4 h-4 inline-block mr-1"
      fill="none"
      height="24"
      stroke="currentColor"
      strokeLinecap="round"
      strokeLinejoin="round"
      strokeWidth="2"
      viewBox="0 0 24 24"
      width="24"
      xmlns="http://www.w3.org/2000/svg"
    >
      <path d="M12 2H2v10l9.29 9.29c.94.94 2.48.94 3.42 0l6.58-6.58c.94-.94.94-2.48 0-3.42L12 2Z" />
      <path d="M7 7h.01" />
    </svg>
  );

    switch (color.toLowerCase()) {
      case "orange":
        return (
          <span className={`px-2 py-1 bg-orange-200 text-orange-800 rounded-md`}>
            <Icon />
            {categoryName}
          </span>
        );
      case "blue":
        return (
          <span className={`px-2 py-1 bg-blue-200 text-blue-800 rounded-md`}>
            <Icon />
            {categoryName}
          </span>
        );
      case "red":
        return (
          <span className={`px-2 py-1 bg-red-200 text-red-800 rounded-md`}>
            <Icon />
            {categoryName}
          </span>
        );
      case "yellow":
        return (
          <span className={`px-2 py-1 bg-yellow-200 text-yellow-800 rounded-md`}>
            <Icon />
            {categoryName}
          </span>
        );
      case "green":
        return (
          <span className={`px-2 py-1 bg-green-200 text-green-800 rounded-md`}>
            <Icon />
            {categoryName}
          </span>
        );
      case "purple":
        return (
          <span className={`px-2 py-1 bg-orange-200 text-orange-800 rounded-md`}>
            <Icon />
            {categoryName}
          </span>
        );
      case "brown":
        return (
          <span className={`px-2 py-1 bg-orange-200 text-orange-800 rounded-md`}>
            <Icon />
            {categoryName}
          </span>
        );

      default:
        return (
          <span className={`px-2 py-1 bg-gray-200 text-gray-800 rounded-md`}>
            <Icon />
            {categoryName}
          </span>
        );
    }
}
