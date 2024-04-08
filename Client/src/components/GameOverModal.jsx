import Modal from './Modal'

function GameOverModal({ showModal, setShowModal, gameStatus, onClose }) {
  return (
    <Modal isOpen={showModal} onClose={() => setShowModal(false)}>
      <div className="flex flex-col space-y-1">
        <h1 className="rounded-lg bg-slate-900 p-6 text-center">
          {gameStatus}
        </h1>
        <button
          className="rounded-md bg-slate-800 px-3.5 py-2.5 text-center text-sm font-semibold text-white shadow-sm hover:bg-slate-700 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600"
          onClick={onClose}
        >
          Close
        </button>
      </div>
    </Modal>
  )
}

export default GameOverModal
