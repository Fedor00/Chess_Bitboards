const Modal = ({ isOpen, onClose, children }) => {
  if (!isOpen) return null

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-50">
      <div className="relative w-full max-w-md rounded-lg bg-stone-500 p-5 text-slate-200 shadow-lg">
        <button
          onClick={onClose}
          className="absolute right-0 top-0 m-2 text-slate-300 hover:text-gray-900"
        >
          &times; {/* This is a simple way to create a close button */}
        </button>
        {children}
      </div>
    </div>
  )
}

export default Modal
