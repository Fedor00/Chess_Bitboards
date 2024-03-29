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
        <h1 className="rounded-lg bg-slate-900 p-4">{text}</h1>
        <button
          onClick={onCopy}
          className="ml-4 rounded bg-slate-900 px-4 py-2 text-white transition duration-200 hover:bg-slate-700"
        >
          {copied ? 'Copied' : 'Copy'}
        </button>
      </div>
    </Modal>
  )
}

export default ShowTextModal
