import { useState } from 'react'
import Modal from './Modal'

function ShowTextModal({ showModal, setShowModal, text }) {
  const [copied, setCopied] = useState(false)
  const onCopy = () => {
    setCopied(true)
    navigator.clipboard
      .writeText(text || '')
      .then(() => {
        setCopied(true)
        setTimeout(() => setCopied(false), 3000)
      })
      .catch((err) => {
        console.error('Failed to copy text: ', err)
      })
  }
  return (
    <Modal isOpen={showModal} onClose={() => setShowModal(false)}>
      <div className="flex flex-row">
        <h1 className="p-4 rounded-lg bg-slate-900">{text}</h1>
        <button
          onClick={onCopy}
          className="px-4 py-2 ml-4 text-white transition duration-200 rounded bg-slate-900 hover:bg-slate-700"
        >
          {copied ? 'Copied' : 'Copy'}
        </button>
      </div>
    </Modal>
  )
}

export default ShowTextModal
