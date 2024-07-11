import Modal from '../Modal'
import { getPathForPiece } from '@/services/Utils'

function PromotionPiece({ showModal, setShowModal, onChoosePiece, color }) {
  return (
    <Modal isOpen={showModal} onClose={() => setShowModal(false)}>
      <div className="flex flex-row justify-center">
        <img
          src={getPathForPiece(color === 'white' ? 'Q' : 'q')}
          alt="Queen"
          className="h-[15%] w-[15%] cursor-pointer object-cover hover:bg-sky-700"
          onClick={() => onChoosePiece(color === 'white' ? 'Q' : 'q')}
        />
        <img
          src={getPathForPiece(color === 'white' ? 'R' : 'r')}
          alt="Queen"
          className="h-[15%] w-[15%] cursor-pointer object-cover  hover:bg-sky-700"
          onClick={() => onChoosePiece(color === 'white' ? 'R' : 'r')}
        />
        <img
          src={getPathForPiece(color === 'white' ? 'B' : 'b')}
          alt="Queen"
          className="h-[15%] w-[15%] cursor-pointer object-cover  hover:bg-sky-700"
          onClick={() => onChoosePiece(color === 'white' ? 'B' : 'b')}
        />
        <img
          src={getPathForPiece(color === 'white' ? 'N' : 'n')}
          alt="Queen"
          className="h-[15%] w-[15%] cursor-pointer object-cover  hover:bg-sky-700"
          onClick={() => onChoosePiece(color === 'white' ? 'N' : 'n')}
        />
      </div>
    </Modal>
  )
}

export default PromotionPiece
